using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using BTHeraldryColorHelper.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace BTHeraldryColorHelper.Patches
{
    /// <summary>
    /// Adds a Heraldry tab to the Crest selection panel in the Customize Company menu
    /// and sets up the UI to toggle between the Crest and Heraldry selectors.
    /// </summary>
    [HarmonyPatch(typeof(HeraldryCreatorPanel), "OnAddedToHierarchy")]
    public static class HeraldryCreatorPanel_OnAddedToHierarchy
    {
        [HarmonyPostfix]
        public static void Postfix(HeraldryCreatorPanel __instance)
        {
            var crestSelector = __instance.transform.Find("Representation/uixPrfPanl_companyCrestSelector");
            if (crestSelector == null) return;

            // Create the Heraldry selector by cloning the Crest selector
            if (__instance.transform.Find("Representation/uixPrfPanl_companyHeraldrySelector") == null)
            {
                var heraldrySelectorGo = Object.Instantiate(crestSelector.gameObject, crestSelector.parent);
                heraldrySelectorGo.name = "uixPrfPanl_companyHeraldrySelector";
                heraldrySelectorGo.SetActive(false);
                var heraldrySelector = heraldrySelectorGo.transform;

                var loadingNotif = heraldrySelector.Find("Representation/loading-Notif")?.gameObject;
                if (loadingNotif != null)
                {
                    var messageText = loadingNotif.transform.Find("loadElementLayout/messageText")?.GetComponent<LocalizableText>();
                    messageText?.text = "LOADING HERALDRIES";
                }

                var pickerWidget = heraldrySelectorGo.AddComponent<HeraldryPickerWidget>();
                var listParent = heraldrySelector.Find("Representation/content-layout/crestsScrollview-list/Viewport/Content-crestItems") as RectTransform;
                pickerWidget.listParent = listParent;
                pickerWidget.loadingNotification = loadingNotif;

                // Set up the tab buttons to switch between Crest and Heraldry selectors
                var crestTextObj = crestSelector.Find("Representation/title-layout/crests_text")?.GetComponent<LocalizableText>();
                var heraldryTextObj = heraldrySelector.Find("Representation/title-layout/crests_text")?.GetComponent<LocalizableText>();
                if (crestTextObj != null && heraldryTextObj != null)
                {
                    crestTextObj.text = "Crests\n<size=50%>Heraldries</size>"; heraldryTextObj.text = "Heraldries\n<size=50%>Crests</size>";

                    var crestTabButton = crestTextObj.GetComponent<Button>() ?? crestTextObj.gameObject.AddComponent<Button>();
                    var heraldryTabButton = heraldryTextObj.GetComponent<Button>() ?? heraldryTextObj.gameObject.AddComponent<Button>();

                    crestTabButton.onClick.AddListener(() =>
                    {
                        crestSelector.gameObject.SetActive(false);
                        heraldrySelector.gameObject.SetActive(true);

                        if (pickerWidget.listParent.childCount == 0)
                        {
                            pickerWidget.SetData(__instance.dataManager, __instance.activeDef?.Description?.Id ?? string.Empty, null);
                        }
                    });

                    heraldryTabButton.onClick.AddListener(() =>
                    {
                        crestSelector.gameObject.SetActive(true);
                        heraldrySelector.gameObject.SetActive(false);
                    });
                }
            }
        }
    }
}