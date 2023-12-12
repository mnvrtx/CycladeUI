using System;
using CycladeUI.Models;
using CycladeUI.Utils.Logging;
using UnityEngine;

namespace CycladeUI.ScriptableObjects
{
    [CreateAssetMenu(fileName = "PopupSystemSettings", menuName = "CycladeUI/PopupSystemSettings", order = 1)]
    public class PopupSystemSettings : ScriptableObject
    {
        private static readonly Log log = new(nameof(PopupSystemSettings));
        public string[] selectedPopupsSerialized;
        public GlobalPopupSystemSettings globalSettings;
        public bool showExitDialogOnEscape;

        [NonSerialized] public PopupLoadEntry[] selectedPopups;

        public void FillFromSerialized()
        {
            selectedPopups = PopupLoadEntry.FromSerialized(selectedPopupsSerialized);
        }
    }
}