using System;
using CycladeUI.Models;
using UnityEngine;

namespace CycladeUI.ScriptableObjects
{
    [CreateAssetMenu(fileName = "PopupSystemSettings", menuName = "CycladeUI/PopupSystemSettings", order = 1)]
    public class PopupSystemSettings : ScriptableObject
    {
        public string[] selectedPopupsSerialized;
        public GlobalPopupSystemSettings globalSettings;
        public bool showExitDialogOnEscape;

        [NonSerialized] public PopupLoadEntry[] selectedPopups;

        public void SetToSerialized()
        {
            selectedPopupsSerialized = PopupLoadEntry.ToSerialized(selectedPopups);
        }

        public void FillFromSerialized()
        {
            selectedPopups = PopupLoadEntry.FromSerialized(selectedPopupsSerialized);
        }
    }
}