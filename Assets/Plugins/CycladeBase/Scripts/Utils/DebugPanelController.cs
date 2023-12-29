using CycladeBase.PerformanceAnalyzer.Trackers;
using TMPro;
using UnityEngine;

namespace CycladeBase.Utils
{
    public class DebugPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TMP_Text btnTxt;
        [SerializeField] private bool initialActive;
        
        [SerializeField] private GameObject[] optionalDebugElements;
        [CycladeHelpBox("Add your debug elements")] public string stub2;
        

        private void Awake()
        {
            UpdateState(initialActive);
        }

        public void U_SwitchPanel()
        {
            var isActive = !panel.activeSelf;
            UpdateState(isActive);
        }

        private void UpdateState(bool isActive)
        {
            panel.SetActive(isActive);
            foreach (var element in optionalDebugElements) 
                element.SetActive(isActive);

            btnTxt.text = isActive ? "Debug: ON" : "Debug: OFF";
        }
    }
}