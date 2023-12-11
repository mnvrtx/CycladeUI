using System;
using CycladeUI.Popups.System;

namespace CycladeUI.Models
{
    [Serializable]
    public class PopupLoadEntry
    {
        public string assemblyName;
        public string typeFullName;
        public string assetPath;
        public PopupLoadType type;

        public PopupLoadEntry()
        {
        }

        public PopupLoadEntry(PopupEntry entry)
        {
            assemblyName = entry.type.assemblyName;
            typeFullName = entry.type.fullName;
            assetPath = entry.assetPath;
        }

        public Type TryFindType() => PopupInfo.TryFind(assemblyName, typeFullName);

        public bool EqualsPopupInfo(PopupInfo entryType)
        {
            return assemblyName == entryType.assemblyName && typeFullName == entryType.fullName;
        }

        public static string[] ToSerialized(PopupLoadEntry[] entries)
        {
            var arr = new string[entries.Length];

            for (int i = 0; i < arr.Length; i++)
                arr[i] = entries[i].ConvertToString();

            return arr;
        }

        public static PopupLoadEntry[] FromSerialized(string[] rawEntries)
        {
            if (rawEntries == null)
                return Array.Empty<PopupLoadEntry>();

            var arr = new PopupLoadEntry[rawEntries.Length];

            for (int i = 0; i < arr.Length; i++)
                arr[i] = ConvertFromString(rawEntries[i]);

            return arr;
        }

        public static PopupLoadEntry ConvertFromString(string s)
        {
            var split = s.Split('|');
            return new PopupLoadEntry
            {
                assemblyName = split[0],
                typeFullName = split[1],
                assetPath = split[2],
                type = Enum.Parse<PopupLoadType>(split[3]),
            };
        }

        public string ConvertToString()
        {
            return $"{assemblyName}|{typeFullName}|{assetPath}|{type}";
        }
    }
}