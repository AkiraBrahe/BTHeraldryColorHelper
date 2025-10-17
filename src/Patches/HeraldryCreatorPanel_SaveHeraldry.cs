using BattleTech;
using BattleTech.UI;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace HeraldryPicker.Patches
{
    /// <summary>
    /// Ensures the correct crest ID is saved when selecting a custom crest.
    /// </summary>
    [HarmonyPatch(typeof(HeraldryCreatorPanel), "SaveHeraldry")]
    public static class HeraldryCreatorPanel_SaveHeraldry
    {
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return new CodeMatcher(instructions, il)
                .MatchForward(false,
                    new CodeMatch(i => i.opcode == OpCodes.Ldfld && i.operand is FieldInfo fi && fi.Name == "crestPicker"),
                    new CodeMatch(i => i.opcode == OpCodes.Callvirt && i.operand is MethodInfo mi && mi.Name == "get_selectedCrestId"))
                .SetOperandAndAdvance(typeof(HeraldryCreatorPanel).GetField("activeDef", BindingFlags.Instance | BindingFlags.NonPublic))
                .SetInstructionAndAdvance(new CodeInstruction(OpCodes.Ldfld, typeof(HeraldryDef).GetField("textureLogoID", BindingFlags.Instance | BindingFlags.Public)))
                .InstructionEnumeration();
        }
    }
}
