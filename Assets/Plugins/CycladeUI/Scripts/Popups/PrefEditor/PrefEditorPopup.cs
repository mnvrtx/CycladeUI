using System;
using CycladeBase.Utils;
using Shared.Utils.Logging;
using CycladeUI.Popups.System;
#if CYCLADEUI_TEXT_MESH_PRO
using TMPro;
#else
using UnityEngine.UI;
#endif

namespace CycladeUI.Popups.PrefEditor
{
    public class PrefEditorPopup : BasePopup
    {
        private static readonly Log log = new(nameof(PrefEditorPopup), CycladeDebugInfo.I);
        
#if CYCLADEUI_TEXT_MESH_PRO
        public TMP_Text title;
#else
        public Text title;
#endif
        public ViewInstances<PrefBlock> prefBlocks;
        
        private object[] _objs;
        private bool _initialExpanded = true;

        private void Awake()
        {
            prefBlocks.Initialize();
        }

        public void Initialize(string titleStr, params object[] objs) => Initialize(titleStr, null, objs);

        public void Initialize(string titleStr, Action onContinue, params object[] objs)
        {
            foreach (var o in objs)
            {
                if (o == null)
                {
                    log.Warn("Didn't open pref editor, cause passed object == null");
                    PopupSystem.ClosePopup(this);
                    return;
                }
            }
            _objs = objs;
            OnClose.Subscribe(onContinue);
            
            title.text = titleStr;

            foreach (var obj in _objs)
                Set(obj, _initialExpanded);
        }

        public PrefEditorPopup SetInitialExpanded(bool expanded)
        {
            _initialExpanded = expanded;
            return this;
        }

        private void Set(object obj, bool expanded)
        {
            prefBlocks.GetNew().Initialize(obj.GetType().Name, obj, expanded);
        }

        public void U_Reset()
        {
            foreach (var prefBlock in prefBlocks.Instances)
                prefBlock.ResetProps();
        }

        public void U_Continue()
        {
            PopupSystem.ClosePopup(this);
        }
    }
}