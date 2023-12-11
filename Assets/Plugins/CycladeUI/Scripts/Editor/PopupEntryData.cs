using System;
using CycladeUI.Models;
using CycladeUI.Popups.System;

namespace CycladeUIEditor
{
    public class PopupEntryData
    {
        public PopupEntry Entry;
        public BasePopup Asset;
        public Type Type;

        public PopupEntryData(PopupEntry entry)
        {
            Entry = entry;
        }
    }
}