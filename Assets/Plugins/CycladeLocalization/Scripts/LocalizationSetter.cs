using System;
using System.Collections.Generic;
using CycladeBase.Utils;
using UnityEngine;

namespace CycladeLocalization
{
    public class LocalizationSetter : MonoBehaviour
    {
        public string area;
        public List<LocInfoKvp> locInfos = new();

        //editor params for editor
        [NonSerialized] public Optional<Enum> SelectedEditorLanguage = new(null);

        private void Awake()
        {
            Localize(area);
        }

        private void Localize(string area)
        {
            foreach (var info in locInfos) 
                info.Text.text = Localization.I.Get(area, info.Key);
        }
    }
}