using System.Collections.Generic;
using System.Linq;
using CycladeBaseEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Solonity.Editor.UtilsModule.Usages
{
    public class UsagesWindow : EditorWindow
    {
        [MenuItem("Window/Cyclade/Usages")]
        public static void ShowWindow()
        {
            GetWindow<UsagesWindow>("Usages");
        }

        private readonly CycladeEditorCommon _editorCommon = new();
        private Vector2 _scrollView = Vector2.zero;
        private GameObject _selected;
        private Scene _selectedScene;
        private bool _includeInactiveComponentsToggle = true;
        private bool _ignoreMe = true;

        private readonly List<UsageData> _usages = new();

        private void OnGUI()
        {
            var scene = CycladeEditorCommon.GetSceneState(out var prefabStage);

            var scTypeName = prefabStage != null ? "prefab" : "scene";
            GUILayout.Label($"Selected {scTypeName} \"{scene.name}\".", EditorStyles.largeLabel);
            
            var active = Selection.activeObject;

            if (active != null && active is GameObject activeGo && activeGo.scene.name != null)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button($"Find \"{activeGo.name}\" references", GUILayout.Height(18)))
                {
                    _selectedScene = scene;
                    _selected = activeGo;
                    _usages.Clear();

                    FindAllReferencesToId(scene, _selected.GetInstanceID(), _selected.name, true);

                    var allComponents = _selected.GetComponents<Component>();

                    foreach (var c in allComponents)
                    {
                        if (!_includeInactiveComponentsToggle)
                        {
                            if (c is Behaviour behaviour && !behaviour.enabled)
                                continue;    
                        }

                        FindAllReferencesToId(scene, c.GetInstanceID(), c.GetType().Name, false);
                    }
                }

                _includeInactiveComponentsToggle = GUILayout.Toggle(_includeInactiveComponentsToggle, new GUIContent("Include inactive", "Include inactive components at selected game object"));
                _ignoreMe = GUILayout.Toggle(_ignoreMe, "Ignore me");
                GUILayout.FlexibleSpace();

                GUILayout.EndHorizontal();
                
                _editorCommon.DrawUILine(Color.gray);
            }
            
            if (_selectedScene != scene)
            {
                _selected = null;
                _selectedScene = default;
            }


            if (_selected != null)
            {
                GUILayout.Label($"Results for \"{_selected.name}\"", EditorStyles.boldLabel);
                
                _scrollView = GUILayout.BeginScrollView(_scrollView);

                var usageDatas = _usages.Where(q => q.Go != null).ToArray();
                
                if (usageDatas.Length > 0)
                {
                    foreach (var usageData in usageDatas)
                    {
                        if (usageData.Go == null)
                            continue;
                        
                        GUILayout.Label(usageData.RefInfo, _editorCommon.RichLabel);
                        GUILayout.Label(usageData.PropName, _editorCommon.RichLabel);

                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Select", GUILayout.MinWidth(1), GUILayout.Height(18)))
                            Selection.activeGameObject = usageData.Go;
                        if (GUILayout.Button("Ping", GUILayout.MinWidth(1), GUILayout.Height(18)))
                            EditorGUIUtility.PingObject(usageData.Go);
                        GUILayout.EndHorizontal();
                        _editorCommon.DrawUILine(Color.gray);
                    }
                }
                else
                {
                    GUILayout.Label($"No references in {_selected.name}");
                }

                GUILayout.EndScrollView();
            }
        }

        public void FindAllReferencesToId(Scene scene, int instanceId, string name, bool isGo)
        {
            var roots = scene.GetRootGameObjects();

            foreach (var root in roots)
            {
                FindInGo(root, instanceId, name, isGo);
            }
        }

        public void FindInGo(GameObject g, int instanceId, string name, bool isGo)
        {
            if (!_ignoreMe || g != _selected)
            {
                var components = g.GetComponents<Component>();

                for (var i = 0; i < components.Length; i++)
                {
                    var component = components[i];
                    if (component == null)
                        continue;

                    SerializedProperty prop = new SerializedObject(component).GetIterator();
                    if (prop.NextVisible(true))
                    {
                        bool enterChild;
                        do
                        {
                            enterChild = prop.propertyType == SerializedPropertyType.Generic;
                            if (prop.propertyType == SerializedPropertyType.ObjectReference)
                            {
                                if (prop.objectReferenceInstanceIDValue == instanceId)
                                {
                                    var usageData = new UsageData
                                    {
                                        RefInfo = $"{(isGo ? "GameObject" : "Component")} <color=orange><b>{name}</b></color> in <b>{g.name}</b> GameObject", //GUILayout.Label();
                                        PropName = $"Set in component <color=cyan>{component.GetType().Name}</color>(index: {i}); Property <b>{prop.name}</b>", //GUILayout.Label();
                                        Go = component.gameObject,
                                    };

                                    _usages.Add(usageData);
                                }
                            }
                        } while (prop.NextVisible(enterChild));
                    }
                }
            }

            foreach (Transform childT in g.transform) {
                FindInGo(childT.gameObject, instanceId, name, isGo);
            }
        }
    }
}