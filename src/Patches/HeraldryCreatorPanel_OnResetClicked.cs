using BattleTech.UI;
using HeraldryPicker.Widgets;

namespace HeraldryPicker.Patches
{
    /// <summary>
    /// Resets the heraldry picker widget when the "Reset" button is clicked.
    /// </summary>
    [HarmonyPatch(typeof(HeraldryCreatorPanel), "OnResetClicked")]
    public static class HeraldryCreatorPanel_OnResetClicked
    {
        [HarmonyPostfix]
        public static void Postfix(HeraldryCreatorPanel __instance)
        {
            var heraldryPickerWidget = __instance.GetComponentInChildren<HeraldryPickerWidget>(true);
            heraldryPickerWidget?.ResetSelection();
        }
    }
}