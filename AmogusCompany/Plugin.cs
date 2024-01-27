using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LC_API.Networking;
using System.Collections.Generic;
using AmogusCompanyMod.Patches;

namespace AmogusCompanyMod {
    [BepInPlugin(modGUID, modName, modVersion)]
    public class AmogusModBase : BaseUnityPlugin {
        private const string modGUID = "takaiten.AmogusCompany";
        private const string modName = "Amogus Company";
        private const string modVersion = "0.0.1";

        private readonly Harmony harmony = new Harmony(modGUID);
        private static AmogusModBase Instance;
        public static ManualLogSource mls;
        public static bool DebugMode = false;

        public static List<ulong> impostorsIDs = new List<ulong>();

        public static ConfigEntry<int> ConfigImpostorCount;
        public static ConfigEntry<bool> ConfigVents;


        void Awake() {
            if (Instance == null) {
                Instance = this;
            }

            ConfigImpostorCount = Config.Bind("General", "ImpostorCount", 1, "Amount of impostors in the game");
            ConfigVents = Config.Bind("General.Toggles", "VentsEnabled", true, "If true, impostor is albe to teleports beetwen vents");

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo(modName + " installed miner successfully...");
            harmony.PatchAll();

            LC_API.GameInterfaceAPI.Events.Handlers.Player.Dying += OtherFunctions.OnDiedCheckForImpostorVictory;
            LC_API.GameInterfaceAPI.Events.Handlers.Player.Left += OtherFunctions.OnLeftCheckForImpostorVictory;

            Network.RegisterAll();
            ConsoleCommands.RegisterConsoleCommands();
        }
    }
}
