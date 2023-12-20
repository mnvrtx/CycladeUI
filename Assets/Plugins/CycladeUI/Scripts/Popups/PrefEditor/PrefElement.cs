using UnityEngine;
using UtilsModule.View.UI.Switchers;

namespace CycladeUI.Popups.PrefEditor
{
    public class PrefElement : MonoBehaviour
    {
        public const int Bool = 0, Value = 1, Enum = 2;

        public SelectableBlock type;
    }
}