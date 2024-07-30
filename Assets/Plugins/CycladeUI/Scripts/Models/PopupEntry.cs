using System;

namespace CycladeUI.Models
{
    [Serializable]
    public class PopupEntry
    {
        public PopupInfo type;
        public string assetPath;

        public PopupEntry(PopupInfo type, string assetPath)
        {
            this.type = type;
            this.assetPath = assetPath;
        }
    }
}