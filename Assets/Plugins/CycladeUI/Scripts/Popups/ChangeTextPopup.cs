using CycladeUI.Popups.System;
#if CYCLADEUI_TEXT_MESH_PRO
using TMPro;
#endif
using UnityEngine.UI;

namespace CycladeUI.Popups
{
    public class ChangeTextPopup : BasePopup
    {
#if CYCLADEUI_TEXT_MESH_PRO
        public TMP_Text title;
        public TMP_Text errorText;
        public TMP_InputField inputField;
#else
        public Text title;
        public Text errorText;
        public InputField inputField;
#endif
        
        public Button okBtn;
    }
}