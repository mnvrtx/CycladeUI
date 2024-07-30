using System.Collections;
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
        [SerializeField] private CanvasGroup[] canvasGroups;

        [CycladeHelpBox("Add your debug elements")]
        public string stub2;

        private void Awake()
        {
            StartCoroutine(WaitAndUpdateState());
        }

        private IEnumerator WaitAndUpdateState()
        {
            yield return null;
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

            foreach (var canvasGroup in canvasGroups)
            {
                canvasGroup.alpha = isActive ? 1 : 0;
                canvasGroup.blocksRaycasts = isActive;
            }

            btnTxt.text = isActive ? "Debug: ON" : "Debug: OFF";
        }
    }
}