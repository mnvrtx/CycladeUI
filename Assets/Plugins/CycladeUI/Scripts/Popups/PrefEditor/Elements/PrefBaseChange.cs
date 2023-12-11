using System.Reflection;

#if CYCLADEUI_TEXT_MESH_PRO
using TMPro;
#else
using UnityEngine.UI;
#endif
using UnityEngine;

namespace CycladeUI.Popups.PrefEditor.Elements
{
    public abstract class PrefBaseChange : MonoBehaviour
    {
#if CYCLADEUI_TEXT_MESH_PRO
        public TMP_Text title;
#else
        public Text title;  
#endif

        public FieldInfo FieldInfo;
        public object Obj;

        public abstract void ResetProperty();
    }
}