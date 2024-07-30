using System;
using System.Text.RegularExpressions;
using CycladeUI.Models;
using Shared.Utils;
using Solonity.View.Utils;
#if CYCLADEUI_TEXT_MESH_PRO
using TMPro;
#endif
using UnityEngine;

namespace CycladeUI.Popups.System
{
    public partial class PopupSystem
    {
        public ChangeTextPopup ShowChangeText(string title, string startText, Action<string> onConfirm, Regex regex = null, string inputEmptyTextHint = "", string errorText = "")
            => ShowChangeText<ChangeTextPopup>(title, startText, onConfirm, regex, inputEmptyTextHint, errorText);

        public T ShowChangeText<T>(string title, string startText, Action<string> onConfirm, Regex regex = null, string inputEmptyTextHint = "", string errorText = "") where T : ChangeTextPopup
        {
            if (regex == null)
                regex = CodeHelpers.GetDefaultValidationRegex();

            if (errorText.IsNullOrEmpty())
                errorText = CodeHelpers.GetDefaultValidationError();

            return ShowPopup<T>(popup =>
            {
                popup.title.text = title;
                popup.errorText.SetActive(false);
#if CYCLADEUI_TEXT_MESH_PRO
                popup.inputField.placeholder.GetComponent<TMP_Text>().text = inputEmptyTextHint;
#endif
                popup.inputField.text = startText;

                popup.inputField.onValueChanged.AddListener(value =>
                {
                    var isEmpty = string.IsNullOrWhiteSpace(value);

                    if (isEmpty || regex != null && !regex.IsMatch(value))
                    {
                        popup.errorText.text = isEmpty ? inputEmptyTextHint : errorText;
                        popup.errorText.color = isEmpty ? Color.white : ViewUtils.ParseHex("D10000");
                        popup.errorText.SetActive(true);
                        popup.okBtn.interactable = false;
                    }
                    else
                    {
                        popup.errorText.SetActive(false);
                        popup.okBtn.interactable = true;
                    }
                });

                popup.okBtn.interactable = !string.IsNullOrEmpty(popup.inputField.text);

                popup.okBtn.onClick.AddListener(() =>
                {
                    var value = popup.inputField.text;

                    if (string.IsNullOrWhiteSpace(value) || popup.errorText.gameObject.activeSelf)
                    {
                        popup.inputField.onValueChanged.Invoke(value);
                        return;
                    }

                    popup.errorText.SetActive(false);
                    if (!string.IsNullOrWhiteSpace(value))
                        onConfirm(value);
                    ClosePopup(popup);
                });
            });
        }

        public ConfirmationPopup ShowConfirmation(string desc, string positiveText, string negative, Action<bool> confirm, PositiveButtonColor positiveBtnColor = PositiveButtonColor.Red, Action close = null)
        {
            return ShowPopup<ConfirmationPopup>(popup =>
            {
                switch (positiveBtnColor)
                {
                    case PositiveButtonColor.Red:
                        popup.btnBackImage.sprite = popup.RedBtnBack;
                        break;
                    case PositiveButtonColor.Green:
                        popup.btnBackImage.sprite = popup.GreenBtnBack;
                        break;
                    case PositiveButtonColor.Blue:
                        popup.btnBackImage.sprite = popup.BlueBtnBack;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(positiveBtnColor), positiveBtnColor, null);
                }

                popup.description.text = desc;
                popup.yesText.text = positiveText;
                popup.okBtn.onClick.AddListener(() =>
                {
                    ClosePopup(popup);
                    confirm(true);
                });

                if (negative.IsNullOrEmpty())
                {
                    popup.noBtn.SetActive(false);
                }
                else
                {
                    popup.noText.text = negative;
                    popup.noBtn.onClick.AddListener(() => { confirm(false); });
                }
            }, close);
        }

        public InfoPopup ShowInfo(string descriptionText, string okText = null, Action onClick = null)
        {
            return ShowPopup<InfoPopup>(popup =>
            {
                popup.description.text = descriptionText;

                if (!string.IsNullOrEmpty(okText))
                    popup.ok.text = okText;

                if (onClick != null)
                    popup.okBtn.onClick.AddListener(onClick.Invoke);
            });
        }
    }
}