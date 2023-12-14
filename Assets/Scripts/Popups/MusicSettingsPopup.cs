using CycladeStorage;
using CycladeUI.Popups.System;
using Solonity.UnityDef.Storage;
using TMPro;
using UnityEngine;

namespace CycladeUIExample.Popups
{
    public class MusicSettingsPopup : BasePopup
    {
        [SerializeField] private TMP_Text btnText;
        
        private void Awake()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            var isMusicEnabled = LocalStorage.I.GetSection<SoundSettings>().IsMusicEnabled;
            btnText.text = isMusicEnabled ? "Turn OFF Music (current: ON)" : "Turn ON Music (current: OFF)";
        }

        public void U_Flip()
        {
            LocalStorage.I.ModifySection<SoundSettings>(q =>
            {
                q.IsMusicEnabled = !q.IsMusicEnabled;
            });
            UpdateText();
        }
    }
}