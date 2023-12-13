using CycladeUI.Popups.System;
#if CYCLADEUI_TEXT_MESH_PRO
using TMPro;
#endif
using UnityEngine.UI;

namespace CycladeUI.Popups
{
    public class ConfirmPopup : BasePopup
    {
#if CYCLADEUI_TEXT_MESH_PRO
        public TMP_Text description;
        public TMP_Text yesText, noText;
#else
        public Text description;
        public Text yesText, noText;
#endif
        
        public Button okBtn, noBtn;
    }
}