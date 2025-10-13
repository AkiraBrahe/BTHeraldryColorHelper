using BattleTech.UI;
using HeraldryPicker.Widgets;

namespace HeraldryPicker.Patches
{
    /// <summary>
    /// Resets the heraldry picker widget when the Customize Company menu is closed.
    /// </summary>
    [HarmonyPatch(typeof(HeraldryCreatorPanel), "OnCancelClicked")]
    public static class HeraldryCreatorPanel_OnCancelClicked
    {
        [HarmonyPostfix]
        public static void Postfix(HeraldryCreatorPanel __instance)
        {
            var heraldryPickerWidget = __instance.GetComponentInChildren<HeraldryPickerWidget>(true);
            heraldryPickerWidget?.ResetSelection();
        }
    }
}