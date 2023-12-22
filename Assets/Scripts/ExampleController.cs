using System.Collections;
using System.Text.RegularExpressions;
using CycladeBase.Utils;
using CycladeBase.Utils.Logging;
using CycladeLocalization;
using CycladeLocalization.Definition;
using CycladeUI.Popups.PrefEditor;
using CycladeUI.Popups.System;
using CycladeUIExample.Models;
using CycladeUIExample.Popups;
using CycladeUIExample.Popups.Shop;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CycladeUIExample
{
    public class ExampleController : MonoBehaviour
    {
        private static readonly Log log = new(nameof(ExampleController));
        
        [SerializeField] private PopupSystem popupSystem;
        [SerializeField] private GameObject interactionLocker;

        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

        private IEnumerator DebugRequestCoroutine()
        {
            interactionLocker.SetActive(true);
            yield return new WaitForSeconds(5);
            interactionLocker.SetActive(false);
        }

        public void U_ShowMusicPopup()
        {
            popupSystem.ShowPopup<MusicSettingsPopup>();
        }

        public void U_ShowNonClosablePopup()
        {
            var infoPopup = popupSystem.ShowInfo("You really won't be able to close this popup :) It might be useful, for example, for a window asking for an app update.")
                .SetNonClosable();
            infoPopup.okBtn.SetActive(false);
        }

        public void U_ShowNonClosableByClickOnBack()
        {
            popupSystem.ShowInfo("This popup cannot be closed by clicking on the outside area. This might be useful, for example, for a popup offering to buy a subscription." + 
                                 $"{Localization.Get(Area.General, "subscriptionDescription")}", okText: Localization.Get(Area.General, "subscriptionConfirm"))
                .SetNonClosableByClickOnBack();
        }

        public void U_TestDebugRequest()
        {
            StartCoroutine(DebugRequestCoroutine());
        }

        public void U_ShowTestPopup()
        {
            popupSystem.ShowFastFollowPopup<ShopPopup>(p =>
            {
                p.Initialize(ProductsData.GetMock());
                p.OnClose.Subscribe(() =>
                {
                    log.Info("ExampleShopPopup closed");
                });
            }).ToUniTask(this).Forget();
        }

        public void U_ShowConfirmationDialog()
        {
            popupSystem.ShowConfirmation("description",
                "yes",
                "no",
                b => Debug.Log($"is confirm? {b}"),
                () => Debug.Log("confirmation closed"));
        }

        public void U_ShowInfo()
        {
            popupSystem.ShowInfo("description",
                "customOk",
                () => Debug.Log("info onClick"));
        }

        public void U_ShowChangeText()
        {
            string pattern = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";
            var regex = new Regex(pattern);
            popupSystem.ShowChangeText("change email",
                "test@mail.com",
                s => Debug.Log($"onConfirm: {s}"),
                regex,
                "enter email",
                "invalid email");
        }

        public void U_ShowPrefEditor()
        {
            var obj1 = new TestModel();
            var obj2 = new AnotherModel();
            var obj3 = new Player();
            popupSystem.ShowPopup<PrefEditorPopup>(popup =>
            {
                popup.SetInitialExpanded(false)
                    .Initialize("PrefEditorTitle", () =>
                    {
                        Debug.Log($"models changed. obj1.network.deep1.deep2.testUshort: {obj1.network.deep1.deep2.testUshort}");
                    }, obj1, obj2, obj3);
            });
        }
        
    }
}