using System;
using TMPro;

namespace CycladeLocalization
{
    [Serializable]
    public class LocInfoKvp
    {
        public TMP_Text Text;
        public string Key;

        [NonSerialized]
        public int KeyIndex = -1;
    }
}