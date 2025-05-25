using BattleTech.Rendering.MechCustomization;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using UnityEngine;
using UnityEngine.UI;

namespace BTHeraldryColorHelper
{
    internal class HeraldryColorIDs
    {
        [HarmonyPatch(typeof(HorizontalScrollSelectorColor), "SetColor")]
        public static class ShowSelectedColorNameInHeader
        {
            [HarmonyPostfix]
            public static void Postfix(int index, ColorSwatch option, HorizontalScrollSelectorColor __instance)
            {
                if (index != 3) return;

                GameObject colorSwatch = __instance.colorTrackers[index].gameObject;
                Transform uixPrfWidget_ColorPanel = colorSwatch?.transform.parent.parent.parent.parent.parent.parent;
                Transform mainColorHeader = uixPrfWidget_ColorPanel?.Find("mainColorHeader");

                if (mainColorHeader != null)
                {
                    var textChild = mainColorHeader.Find("text");
                    if (textChild != null)
                    {
                        var headerText = textChild.GetComponent<LocalizableText>();
                        if (headerText != null)
                        {
                            headerText.text = $"{headerText.text.Split(':')[0]}: {option.name}";
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(HorizontalScrollSelectorColor), "SetColor")]
        public static class ShowColorNameOnEachSwatch
        {
            [HarmonyPostfix]
            public static void Postfix(int index, ColorSwatch option, HorizontalScrollSelectorColor __instance)
            {
                if (!Main.Settings.ShowColorNameOnEachSwatch) return;

                GameObject colorSwatch = __instance.colorTrackers[index].gameObject;
                Text textComponent = colorSwatch?.GetComponentInChildren<Text>();

                if (textComponent == null)
                {
                    GameObject textWidget = new("ColorID");
                    textWidget.transform.SetParent(colorSwatch.transform, false);

                    textComponent = textWidget.AddComponent<Text>();
                    textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    textComponent.fontStyle = FontStyle.Bold;
                    textComponent.fontSize = 10;
                    textComponent.alignment = TextAnchor.MiddleCenter;
                    textComponent.verticalOverflow = VerticalWrapMode.Overflow;

                    var outline = textComponent.gameObject.AddComponent<Outline>();
                    outline.effectColor = Color.black;
                    outline.effectDistance = new Vector2(1f, -1f);

                    RectTransform rectTransform = textComponent.GetComponent<RectTransform>();
                    RectTransform parentRect = colorSwatch.transform.parent.GetComponent<RectTransform>();
                    rectTransform.anchorMin = Vector2.zero; rectTransform.anchorMax = Vector2.one;
                    rectTransform.offsetMin = Vector2.zero; rectTransform.offsetMax = Vector2.zero;
                    rectTransform.sizeDelta = parentRect.sizeDelta - new Vector2(5f, 5f);
                }

                string displayText;
                if (option.name.StartsWith("Bright"))
                {
                    var parts = option.name.Split('_');
                    string colorName = parts[0].Replace("Bright", "").Substring(0, 2);
                    displayText = colorName + parts[1];
                }
                else
                {
                    displayText = option.name.Substring(option.name.IndexOf('_') + 1);
                }

                textComponent.text = displayText;
            }
        }
    }
}
