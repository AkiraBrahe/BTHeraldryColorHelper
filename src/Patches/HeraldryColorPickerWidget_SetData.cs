using BattleTech.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeraldryPicker.Patches
{
    /// <summary>
    /// Fixes an issue where the HeraldryColorPickerWidget would not properly refresh its color options.
    /// </summary>
    [HarmonyPatch(typeof(HeraldryColorPickerWidget), "SetData")]
    public static class HeraldryColorPickerWidget_SetData
    {
        private static readonly Dictionary<HeraldryColorPickerWidget, Coroutine> runningCoroutines = [];

        [HarmonyPrefix]
        public static bool Prefix(HeraldryColorPickerWidget __instance, string selectedPrimaryColorId, string selectedSecondaryColorId, string selectedAccentColorId, UnityEngine.Events.UnityAction OnColorChanged)
        {
            if (runningCoroutines.TryGetValue(__instance, out var coroutine) && coroutine != null)
            {
                __instance.StopCoroutine(coroutine);
                runningCoroutines.Remove(__instance);
            }

            __instance.selectedPrimaryColorId = selectedPrimaryColorId;
            __instance.selectedSecondaryColorId = selectedSecondaryColorId;
            __instance.selectedAccentColorId = selectedAccentColorId;
            __instance.OnColorChangedCB = OnColorChanged;

            Coroutine newCoroutine = __instance.StartCoroutine(ResetColorOptions(__instance, __instance.LoadColorOptions));
            runningCoroutines[__instance] = newCoroutine;

            return false;
        }

        // Custom coroutine that matches the original logic
        private static IEnumerator ResetColorOptions(HeraldryColorPickerWidget widget, UnityEngine.Events.UnityAction onComplete = null)
        {
            var primary = widget.primaryColorPicker;
            var secondary = widget.secondaryColorPicker;
            var accent = widget.accentColorPicker;

            primary.onValueChanged = (UnityEngine.Events.UnityAction)System.Delegate.Remove(primary.onValueChanged, new UnityEngine.Events.UnityAction(widget.RefreshSelectedColors));
            secondary.onValueChanged = (UnityEngine.Events.UnityAction)System.Delegate.Remove(secondary.onValueChanged, new UnityEngine.Events.UnityAction(widget.RefreshSelectedColors));
            accent.onValueChanged = (UnityEngine.Events.UnityAction)System.Delegate.Remove(accent.onValueChanged, new UnityEngine.Events.UnityAction(widget.RefreshSelectedColors));
            primary.ClearOptions();
            secondary.ClearOptions();
            accent.ClearOptions();
            widget.selectedPrimaryColor = HeraldryColorPickerWidget.primarySwatches[0];
            widget.selectedSecondaryColor = HeraldryColorPickerWidget.secondarySwatches[0];
            widget.selectedAccentColor = HeraldryColorPickerWidget.accentSwatches[0];
            yield return new WaitForSeconds(0.5f);
            onComplete?.Invoke();

            runningCoroutines.Remove(widget);
        }
    }
}