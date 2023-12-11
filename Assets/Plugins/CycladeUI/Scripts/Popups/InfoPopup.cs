using CycladeUI.Popups.System;
#if CYCLADEUI_TEXT_MESH_PRO
using TMPro;
#endif
using UnityEngine.UI;

namespace CycladeUI.Popups
{
    public class InfoPopup : BasePopup
    {
#if CYCLADEUI_TEXT_MESH_PRO
        public TMP_Text ok, description;
#else
        public Text ok, description;
#endif
        public Button okBtn;
    }
}