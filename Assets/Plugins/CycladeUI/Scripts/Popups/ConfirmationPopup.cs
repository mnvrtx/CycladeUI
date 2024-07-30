using CycladeUI.Popups.System;
using UnityEngine;
#if CYCLADEUI_TEXT_MESH_PRO
using TMPro;
#endif
using UnityEngine.UI;

namespace CycladeUI.Popups
{
    public class ConfirmationPopup : BasePopup
    {
        public Sprite RedBtnBack;
        public Sprite GreenBtnBack;
        public Sprite BlueBtnBack;
        
        public Image btnBackImage;

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