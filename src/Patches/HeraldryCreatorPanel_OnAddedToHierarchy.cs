using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using HeraldryPicker.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace HeraldryPicker.Patches
{
    #region Heraldry Tab

    /// <summary>
    /// Adds a Heraldry tab to the Crest selector in the Customize Company menu
    /// and sets up the UI to toggle between the Crest and Heraldry selectors.
    /// </summary>
    [HarmonyPatch(typeof(HeraldryCreatorPanel), "OnAddedToHierarchy")]
    public static class HeraldryCreatorPanel_OnAddedToHierarchy_HeraldryTab
    {
        [HarmonyPostfix]
        public static void Postfix(HeraldryCreatorPanel __instance)
        {
            var crestSelector = __instance.crestPicker.gameObject;
            if (crestSelector.transform.parent.Find("uixPrfPanl_companyHeraldrySelector") == null)
            {
                var heraldrySelector = Object.Instantiate(crestSelector.gameObject, crestSelector.transform.parent);
                heraldrySelector.name = "uixPrfPanl_companyHeraldrySelector";
                heraldrySelector.SetActive(false);

                var loadingNotif = heraldrySelector.transform.Find("Representation/loading-Notif")?.gameObject;
                if (loadingNotif != null)
                {
                    var messageText = loadingNotif.transform.Find("loadElementLayout/messageText")?.GetComponent<LocalizableText>();
                    messageText?.text = "LOADING HERALDRIES";
                }

                var pickerWidget = heraldrySelector.AddComponent<HeraldryPickerWidget>();
                pickerWidget.listParent = heraldrySelector.transform.Find("Representation/content-layout/crestsScrollview-list/Viewport/Content-crestItems") as RectTransform;
                pickerWidget.loadingNotification = loadingNotif;

                SetupTabs(crestSelector, heraldrySelector, pickerWidget, __instance);
            }
        }

        private static void SetupTabs(GameObject crestSelector, GameObject heraldrySelector, HeraldryPickerWidget pickerWidget, HeraldryCreatorPanel panelInstance)
        {
            var crestText = crestSelector.transform.Find("Representation/title-layout/crests_text")?.GetComponent<LocalizableText>();
            var heraldryText = heraldrySelector.transform.Find("Representation/title-layout/crests_text")?.GetComponent<LocalizableText>();

            if (crestText != null && heraldryText != null)
            {
                crestText.text = "Crests\n<size=50%>Heraldries</size>";
                heraldryText.text = "Heraldries\n<size=50%>Crests</size>";

                var crestTabButton = crestText.GetComponent<Button>() ?? crestText.gameObject.AddComponent<Button>();
                crestTabButton.onClick.RemoveAllListeners();
                crestTabButton.onClick.AddListener(() =>
                {
                    crestSelector.SetActive(false);
                    heraldrySelector.SetActive(true);

                    if (pickerWidget.listParent.childCount == 0)
                    {
                        pickerWidget.SetData(panelInstance.dataManager, panelInstance.activeDef?.Description?.Id ?? string.Empty, null);
                    }
                });

                var heraldryTabButton = heraldryText.GetComponent<Button>() ?? heraldryText.gameObject.AddComponent<Button>();
                heraldryTabButton.onClick.RemoveAllListeners();
                heraldryTabButton.onClick.AddListener(() =>
                {
                    crestSelector.SetActive(true);
                    heraldrySelector.SetActive(false);
                });
            }
        }
    }

    #endregion
}