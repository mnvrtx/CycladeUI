using System.Text.RegularExpressions;
using CycladeUI.Popups.PrefEditor;
using CycladeUI.Popups.System;
using UnityEngine;

namespace CycladeUI.Test
{
    public class ExampleController : MonoBehaviour
    {
        [SerializeField] private PopupSystem popupSystem;

        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

        public void U_ShowConfirmationDialog()
        {
            popupSystem.ShowConfirmation("description",
                "yes",
                "no",
                b => Debug.Log($"is confirm? {b}"),
                () => Debug.Log("confirmation closed"));
        }

        public void U_ShowInfo()
        {
            popupSystem.ShowInfo("description",
                "customOk",
                () => Debug.Log("info onClick"));
        }

        public void U_ShowChangeText()
        {
            string pattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";
            var regex = new Regex(pattern);
            popupSystem.ShowChangeText("change email",
                "test@mail.com",
                s => Debug.Log($"onConfirm: {s}"),
                regex,
                "enter email",
                "invalid email");
        }

        public void U_ShowPrefEditor()
        {
            var obj1 = new TestModel();
            var obj2 = new AnotherModel();
            var obj3 = new Player();
            popupSystem.ShowPopup<PrefEditorPopup>(popup => popup.SetInitialExpanded(false).Initialize("PrefEditorTitle", () =>
            {
                Debug.Log($"models changed. obj1.network.deep1.deep2.testUshort: {obj1.network.deep1.deep2.testUshort}");
            }, obj1, obj2, obj3));
        }
        
    }
}