using System;
using System.Text.RegularExpressions;
using CycladeUI.Utils;
using UnityEngine;

namespace CycladeUI.Popups.System
{
    public partial class PopupSystem
    {
        public ChangeTextPopup ShowChangeText(string title, string startText, Action<string> onConfirm, Regex regex = null, string inputText = "", string errorText = "")
        {
            return ShowPopup<ChangeTextPopup>(popup =>
            {
                popup.title.text = title;
                popup.errorText.gameObject.SetActive(false);

                popup.inputField.text = startText;
                popup.inputField.onValueChanged.AddListener(value => 
                {
                    var isEmpty = string.IsNullOrWhiteSpace(value);

                    if (isEmpty || regex != null && !regex.IsMatch(value))
                    {
                        popup.errorText.text = isEmpty ? inputText : errorText;
                        popup.errorText.gameObject.SetActive(true);
                    }
                    else
                    {
                        popup.errorText.gameObject.SetActive(false);
                    }
                });
                
                popup.okBtn.onClick.AddListener(() =>
                {
                    var value = popup.inputField.text;

                    if (string.IsNullOrWhiteSpace(value) || popup.errorText.gameObject.activeSelf)
                    {
                        popup.inputField.onValueChanged.Invoke(value);
                        return;
                    }

                    popup.errorText.gameObject.SetActive(false);
                    onConfirm(value);
                    ClosePopup(popup);
                });
                popup.GetComponent<RectTransform>().ToInitial();
            });
        }

        public ConfirmationPopup ShowConfirmation(string desc, string yes, string no, Action<bool> confirm, Action close = null)
        {
            return ShowPopup<ConfirmationPopup>(popup =>
            {
                popup.description.text = desc;
                popup.yesText.text = yes;
                popup.noText.text = no;
                popup.okBtn.onClick.AddListener(() =>
                {
                    confirm(true);
                    ClosePopup(popup);
                });
                popup.noBtn.onClick.AddListener(() =>
                {
                    confirm(false);
                });
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