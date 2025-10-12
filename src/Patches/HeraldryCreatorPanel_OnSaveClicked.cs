namespace HeraldryPicker.Patches
{
    /// <summary>
    /// After clicking "Save", update the activeDef values to match the color and crest pickers.
    /// </summary>
    [HarmonyPatch(typeof(BattleTech.UI.HeraldryCreatorPanel), "OnSaveClicked")]
    public static class HeraldryCreatorPanel_OnSaveClicked
    {
        [HarmonyPrefix]
        public static void Prefix(BattleTech.UI.HeraldryCreatorPanel __instance)
        {
            if (__instance.activeDef != null)
            {
                __instance.activeDef.primaryMechColorID = __instance.colorPicker.selectedPrimaryColorId;
                __instance.activeDef.secondaryMechColorID = __instance.colorPicker.selectedSecondaryColorId;
                __instance.activeDef.tertiaryMechColorID = __instance.colorPicker.selectedAccentColorId;
                __instance.activeDef.textureLogoID = __instance.crestPicker.selectedCrestId;
                __instance.activeDef.Refresh();
            }
        }
    }
}