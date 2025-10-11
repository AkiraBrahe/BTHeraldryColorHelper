using HBS.Logging;
using Newtonsoft.Json;
using System;

namespace BTHeraldryColorHelper
{
    public class Main
    {
        private const string ModName = "BTHeraldryColorHelper";
        private const string HarmonyInstanceId = "com.github.AkiraBrahe.BTHeraldryColorHelper";

        internal static Harmony harmony;
        internal static ILog Log { get; private set; }
        internal static ModSettings Settings { get; private set; }

        public class ModSettings
        {
            public bool ShowColorNameOnEachSwatch { get; set; } = true;
        }

        public static void Init(string settingsJSON)
        {
            Log = Logger.GetLogger(ModName);
            Logger.SetLoggerLevel(ModName, LogLevel.Debug);

            try
            {
                Settings = JsonConvert.DeserializeObject<ModSettings>(settingsJSON) ?? new ModSettings();
                harmony = new Harmony(HarmonyInstanceId);
                harmony.PatchAll();
                Log.Log("Mod initialized!");
            }
            catch (Exception ex)
            {
                Log.LogException(ex);
            }
        }
    }
}
