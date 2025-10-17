using BattleTech;
using BattleTech.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HeraldryPicker.Utils
{
    internal class HeraldryExporter
    {
        private static bool exported = false;

        /// <summary>
        /// Exports all heraldries present in the game to a dictionary-style format.
        /// </summary>
        public static void ExportHeraldries(DataManager dataManager)
        {
            if (exported) return;

            try
            {
                var filterGroupDir = System.IO.Path.Combine(Main.ModDir, "FilterGroups");
                System.IO.Directory.CreateDirectory(filterGroupDir);
                var exportPath = System.IO.Path.Combine(filterGroupDir, "AllHeraldries.json");

                var allHeraldryDefs = new List<HeraldryDef>();
                foreach (var kvp in dataManager.Heraldries)
                {
                    var def = kvp.Value;
                    if (def != null)
                    {
                        allHeraldryDefs.Add(def);
                    }
                }

                var heraldryList = allHeraldryDefs.Select(h => new
                {
                    Name = Regex.Replace(h.Description.Name, @"(\p{Ll})(\p{Lu})", "$1 $2"),
                    Id = h.Description.Id.Replace("heraldrydef_", ""),
                    Group = vanillaHeraldries.Contains(h.Description.Id.Replace("heraldrydef_", "")) ? "Vanilla" : ""
                }).OrderBy(h => h.Name).ThenBy(h => h.Id, new NaturalStringComparer());

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(heraldryList, Newtonsoft.Json.Formatting.Indented);
                System.IO.File.WriteAllText(exportPath, json);
                Main.Log.LogDebug($"Exported {dataManager.Heraldries.Count} heraldries to {exportPath}");
            }
            catch (System.Exception ex)
            {
                Main.Log.LogException("Failed to export heraldries.", ex);
            }
            finally { exported = true; }
        }

        internal static readonly HashSet<string> vanillaHeraldries =
        [
            "BaumannGroup", "Betrayers", "BlackCalderaDefense", "BlackWidowCompany", "BountyHunterAssociates", "canopus", "career_freelancer", "career_gladiator", "career_mercenary", "career_merchantguard",
            "career_pirate", "career_soldier", "davion", "default", "directorate", "DuchyOfAndurien", "EmeraldDawn", "enemy", "GrayDeathLegion", "hostileMercs",
            "KellHounds", "kurita", "liao", "locals", "MajestyMetals", "marik", "MasonsMarauders", "nautilus", "none", "PaladinProtectionLLC",
            "pirate01", "pirate02", "pirate03", "pirate04", "pirate05", "pirates", "player", "ProfHorvat", "random01", "random02",
            "random03", "random04", "random05", "RazorbackMercs", "RedHareRegiment", "restoration", "SecuritySolutionsInc", "SianTriumphant", "sldf", "sldf_OD",
            "SteelBeast", "steiner", "taurian", "Template", "yangBigScore_stolen"
        ];
    }
}
