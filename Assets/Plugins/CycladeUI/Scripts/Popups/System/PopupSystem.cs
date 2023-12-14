using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using CycladeUI.Models;
using CycladeUI.ScriptableObjects;
using CycladeUI.Utils;
using CycladeUI.Utils.Logging;
using UnityEngine;
using UnityEngine.UI;

namespace CycladeUI.Popups.System
{
    public partial class PopupSystem : MonoBehaviour
    {
        private UiLog log;

        public int OpenedPopupsCount => _stack.Count;
        public DebugSafeAreaSettings DebugSafeArea => settings.globalSettings.debugSafeAreaSettings;
        public IReadOnlyList<BasePopup> Stack => _stack;

        [SerializeField] private bool isNeedToLogInfo = true;
        [SerializeField] private PopupSystemSettings settings;
        [SerializeField] private RectTransform backgroundTemplate;
        [SerializeField] private RectTransform active;
        [SerializeField] private PopupSystemLogicBase optionalLogic;

        private Canvas _canvas;

        private readonly Dictionary<Type, PopupLoadEntry> _entries = new();
        private readonly Dictionary<Type, BasePopup> _loadedPopups = new();

        private readonly List<BasePopup> _stack = new();

        private void Awake()
        {
            log = new UiLog($"{gameObject.name}");

            if (settings == null)
            {
                log.Error($"Not initialized. Settings is not set");
                return;
            }

            _canvas = transform.root.GetComponent<Canvas>();
            if (_canvas == null)
            {
                log.Error($"Not initialized. Root is not canvas");
                return;
            }

            var stopWatch = Stopwatch.StartNew();

            settings.FillFromSerialized();

            var assemblies = settings.globalSettings.assemblies.Select(Assembly.Load)
                .ToDictionary(q => q.FullName, q => q);

            foreach (var load in settings.selectedPopups)
            {
                var assembly = assemblies[load.assemblyName];
                _entries.Add(assembly.GetType(load.typeFullName), load);
            }

            foreach (var kvp in _entries)
            {
                var type = kvp.Key;
                var load = kvp.Value;

                if (load.type == PopupLoadType.FastFollow)
                    continue;

                if (load.type == PopupLoadType.Preload)
                {
                    _loadedPopups.Add(type, PopupLoader.Load(type, load.assetPath));
                    log.Debug($"Loaded: {type.Name}", isNeedToLogInfo);
                }
            }

            StartCoroutine(LoadingFastFollow());

            log.Debug($"Loading complete. Total elapsed: {stopWatch.ElapsedMilliseconds}ms", isNeedToLogInfo);
        }

        public IEnumerator LoadingFastFollow()
        {
            foreach (var kvp in _entries)
            {
                var type = kvp.Key;
                var load = kvp.Value;

                if (load.type != PopupLoadType.FastFollow)
                    continue;

                var sw = Stopwatch.StartNew();
                var request = PopupLoader.LoadAsync(type, load.assetPath);
                while (!request.isDone)
                    yield return null;
                _loadedPopups.Add(type, (BasePopup)request.asset);
                log.Debug($"FastFollow. Loaded: {type.Name}. Elapsed: {sw.ElapsedMilliseconds}ms", isNeedToLogInfo);
            }
        }

        public T ShowPopup<T>(Action<T> onCreate = null, Action onClose = null) where T : BasePopup
        {
            var type = typeof(T);

            ThrowIfNotRegistered(type);

            if (_entries[type].type == PopupLoadType.FastFollow)
                throw new Exception($"Use {nameof(ShowFastFollowPopup)} method for {type.Name}, because type is FastFollow");

            return ShowPopupInternal(onCreate, onClose);
        }

        public IEnumerator<T> ShowFastFollowPopup<T>(Action<T> onCreate = null, Action onClose = null) where T : BasePopup
        {
            var type = typeof(T);

            ThrowIfNotRegistered(type);

            if (_entries[type].type != PopupLoadType.FastFollow)
                throw new Exception($"Use {nameof(ShowPopup)} method for {type.Name}, because type is not FastFollow");

            while (!_loadedPopups.ContainsKey(type))
                yield return null;

            yield return ShowPopupInternal(onCreate, onClose);
        }

        public void ClosePopup(BasePopup popup)
        {
            if (!_stack.Contains(popup))
                return;

            if (popup.NonClosable)
            {
                log.Info($"{popup.name} is NonClosable");
                return;
            }

            _stack.Remove(popup);
            OnClosePopup(popup);
            SetLastActive();
        }

        public void CloseLast()
        {
            if (_stack.Count == 0)
            {
                log.Warn($"Don't have any popups to close");
                return;
            }

            if (_stack.Last().NonClosable)
            {
                log.Info($"{_stack.Last().name} is NonClosable");
                return;
            }

            var popup = _stack.Pop();
            OnClosePopup(popup);
            SetLastActive();
        }

        private void SetLastActive()
        {
            if (_stack.Count > 0)
                _stack.Last().SetActiveDelayed.Begin(0, true);
        }

        public void ClosePopupsOfType<T>() where T : BasePopup
        {
            var type = typeof(T);
            for (var i = 0; i < _stack.Count; i++)
            {
                if (_stack[i].GetType() != type)
                    continue;

                ClosePopup(_stack[i]);
                i--;
            }
        }

        public void CloseAllPopups()
        {
            for (var i = 0; i < _stack.Count; i++)
            {
                ClosePopup(_stack[i]);
                i--;
            }
        }

        public BasePopup TryToFindOpenedPopup<T>() where T : BasePopup
        {
            var type = typeof(T);
            return _stack.FirstOrDefault(x => x.GetType() == type);
        }

        private T ShowPopupInternal<T>(Action<T> onCreate = null, Action onClose = null) where T : BasePopup
        {
            var type = typeof(T);

            var template = GetTemplate(type);

            var holder = new GameObject($"{type.Name}", typeof(RectTransform)).GetComponent<RectTransform>();
            holder.SetParent(active, false);
            holder.anchorMin = Vector2.zero;
            holder.anchorMax = Vector2.one;
            holder.offsetMin = Vector2.zero;
            holder.offsetMax = Vector2.zero;

            var popup = Instantiate(template, holder);
            popup.PopupSystem = this;
            popup.Holder = holder;

            if (popup.needSafeArea)
                popup.GetComponent<RectTransform>().FitInSafeArea(settings.globalSettings.debugSafeAreaSettings);

            if (popup.needBackground)
            {
                var bg = Instantiate(backgroundTemplate, holder);
                AddClosingEvents(popup, bg.GetComponent<Button>(), true);
                bg.SetAsFirstSibling();
                if (bg.TryGetComponent(out Image image)) 
                    image.color = popup.backgroundColor;
                bg.gameObject.SetActive(true);
            }

            if (popup.optionalCloseBtn != null)
                AddClosingEvents(popup, popup.optionalCloseBtn, false);

            float animationDelay = 0;
            if (popup.optionalAnimation != null)
                animationDelay = popup.optionalAnimation.PlayForward();

            if (onClose != null)
                popup.OnClose.Subscribe(onClose);

            popup.SetActiveDelayed = holder.gameObject.AddComponent<SetActiveDelayed>();

            if (_stack.Count > 0)
            {
                if (popup.isFullScreenPopup)
                {
                    foreach (var p in _stack) 
                        p.SetActiveDelayed.Begin(animationDelay, false);
                }
            }

            _stack.Add(popup);

            var typedWindow = (T)popup;
            log.Debug($"Show popup {holder.name}", isNeedToLogInfo);
            try
            {
                onCreate?.Invoke(typedWindow);
            }
            catch (Exception e)
            {
                log.Exception(e);
            }
            return typedWindow;
        }

        private void Update()
        {
            CheckEscape();
        }

        private void CheckEscape()
        {
            var haveLocker = optionalLogic != null && optionalLogic.IsLocked();

            if (Input.GetKeyDown(KeyCode.Escape) && !haveLocker)
            {
                if (_stack.Count > 0)
                {
                    CloseLast();
                }
                else
                {
                    if (settings.showExitDialogOnEscape)
                    {
                        if (optionalLogic != null)
                            optionalLogic.ShowExitDialogOnBack(this);
                        else
                            log.Warn("Try to ShowExitDialogOnBack, but Logic is null.");
                    }
                }
            }
        }

        private void ThrowIfNotRegistered(Type type)
        {
            if (!_entries.ContainsKey(type))
                throw new Exception($"Not found {type.Name}. Please add it in PopupSettings scriptable object");
        }

        private BasePopup GetTemplate(Type type)
        {
            if (!_loadedPopups.TryGetValue(type, out var prefab))
            {
                var load = _entries[type];

                if (load.type == PopupLoadType.OnDemand)
                {
                    var sw = Stopwatch.StartNew();
                    prefab = PopupLoader.Load(type, load.assetPath);
                    _loadedPopups.Add(type, prefab);
                    log.Debug($"OnDemand. Loaded: {type.Name}. Elapsed: {sw.ElapsedMilliseconds}ms", isNeedToLogInfo);
                }
                else
                {
                    throw new Exception($"Popup {type.Name}({load.assetPath}) is not loaded.");
                }
            }

            return prefab;
        }

        private void AddClosingEvents(BasePopup popup, Button btn, bool isBackground)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                if (isBackground && popup.NonClosableByClickOnBack)
                    return;

                ClosePopup(popup);
            });
        }

        private void OnClosePopup(BasePopup popup)
        {
            var holder = popup.Holder.gameObject;
            log.Debug($"Close popup {holder.name}", isNeedToLogInfo);
            var destroyDelay = popup.optionalAnimation != null ? popup.optionalAnimation.PlayBackward() : 0;

            if (destroyDelay > 0)
            {
                if (popup.optionalCanvasGroup != null)
                    popup.optionalCanvasGroup.interactable = false;
                else
                    log.Warn("The OptionalCanvasGroup is null. Please set it if you wish to disable buttons during animations.");
            }

            Destroy(holder, destroyDelay);
            popup.OnClose.InvokeAll();
        }
    }
}