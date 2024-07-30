using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CycladeBaseEditor
{
    public class UITestWindow : EditorWindow
    {
        private float sliderValue = 0.5f;
        private Image[] mockupObjects;
        private Scene currentScene;
        
        [MenuItem("Window/Cyclade/UITestWindow")]
        public static void ShowWindow()
        {
            GetWindow<UITestWindow>("UITestWindow");
        }

        private void OnEnable()
        {
            // Cache all GameObjects named 'MOCKUP' when the window is enabled
            CacheMockupObjects();
        }

        private void OnGUI()
        {
            GUILayout.Label("Set Transparency for Images named 'MOCKUP'", EditorStyles.boldLabel);

            // Add a slider
            float newSliderValue = EditorGUILayout.Slider("Transparency", sliderValue, 0, 1);
            if (newSliderValue != sliderValue)
            {
                sliderValue = newSliderValue;
                ApplyTransparencyToImages();
            }
            
            if (GUILayout.Button("Refresh Cache"))
            {
                CacheMockupObjects();
            }
        }

        private void CacheMockupObjects()
        {
            // Cache the current scene
            currentScene = SceneManager.GetActiveScene();

            // Find and cache all GameObjects named 'MOCKUP'
            mockupObjects = GameObject.FindObjectsOfType<Image>();

            // Filter to only include those named 'MOCKUP'
            mockupObjects = System.Array.FindAll(mockupObjects, obj => obj.name.StartsWith("MOCKUP"));
        }

        private void ApplyTransparencyToImages()
        {
            // Check if the scene has changed or cache is empty
            if (mockupObjects == null || currentScene != SceneManager.GetActiveScene())
            {
                CacheMockupObjects();
            }

            foreach (var img in mockupObjects)
            {
                if (img == null)
                {
                    CacheMockupObjects();
                }

                Color color = img.color;
                color.a = sliderValue;
                img.color = color;
            }
            SceneView.RepaintAll();

        }
    }
}