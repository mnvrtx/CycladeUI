using CycladeUI.Popups.System;
using UnityEngine;

namespace CycladeUI.Test
{
    public class SampleController : MonoBehaviour
    {
        [SerializeField] private PopupSystem popupSystem;

        public void U_OpenDialog()
        {
            popupSystem.ShowConfirmation("description",
                "yes",
                "no",
                b => Debug.Log($"is confirm? {b}"),
                () => Debug.Log("confirmation closed"));
        }
        
    }
}