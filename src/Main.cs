using Newtonsoft.Json;

namespace BTHeraldryColorHelper
{
    public class Main
    {
        internal static Harmony harmony;
        internal static ModSettings Settings { get; private set; }

        public class ModSettings
        {
            public bool ShowColorNameOnEachSwatch { get; set; } = true;
        }

        public static void Init(string settingsJSON)
        {
            Settings = JsonConvert.DeserializeObject<ModSettings>(settingsJSON) ?? new ModSettings();
            harmony = new Harmony("com.github.AkiraBrahe.BTHeraldryColorHelper");
            harmony.PatchAll();
        }
    }
}
