using BattleTech.UI;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace HeraldryPicker.Patches
{
    /// <summary>
    /// Prevents RefreshHeraldry from overwriting a custom crest by wrapping the crest assignment in a null check.
    /// </summary>
    [HarmonyPatch(typeof(HeraldryCreatorPanel), "RefreshHeraldry")]
    public static class HeraldryCreatorPanel_RefreshHeraldry
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return new CodeMatcher(instructions, il)
                .Advance(6).CreateLabel(out var skipLabel)
                .Start().InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(HeraldryCreatorPanel).GetField("crestPicker", BindingFlags.Instance | BindingFlags.NonPublic)),
                    new CodeInstruction(OpCodes.Ldfld, typeof(HeraldryCrestPickerWidget).GetField("selectedCrest", BindingFlags.Instance | BindingFlags.NonPublic)),
                    new CodeInstruction(OpCodes.Brfalse, skipLabel))
                .InstructionEnumeration();
        }
    }
}