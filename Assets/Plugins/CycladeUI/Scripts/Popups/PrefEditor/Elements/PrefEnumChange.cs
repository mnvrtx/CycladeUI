using System;
#if CYCLADEUI_TEXT_MESH_PRO
using TMPro;
#else
using UnityEngine.UI;
#endif

namespace CycladeUI.Popups.PrefEditor.Elements
{
    public class PrefEnumChange : PrefBaseChange
    {
#if CYCLADEUI_TEXT_MESH_PRO
        public TMP_Dropdown dropdown;
#else
        public Dropdown dropdown;
#endif
        
        [NonSerialized] public int InitialValue;

        public void U_OnChange()
        {
            var val = dropdown.value;

            var type = Enum.GetUnderlyingType(FieldInfo.FieldType);
            var valTyped = Convert.ChangeType(val, type);
            FieldInfo.SetValue(Obj, valTyped);
        }

        public override void ResetProperty()
        {
            dropdown.value = InitialValue;
            U_OnChange();
        }
    }
}