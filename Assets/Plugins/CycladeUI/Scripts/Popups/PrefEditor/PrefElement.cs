using CycladeBase.Utils.Switchers;
using UnityEngine;

namespace CycladeUI.Popups.PrefEditor
{
    public class PrefElement : MonoBehaviour
    {
        public const int Bool = 0, Value = 1, Enum = 2;

        public SelectableBlock type;
    }
}