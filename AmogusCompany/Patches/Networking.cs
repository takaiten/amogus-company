using LC_API.GameInterfaceAPI.Features;
using LC_API.Networking;
using LCShrinkRay.comp;
using System.Collections.Generic;

internal class Networking {
    public List<ulong> ImpostorList { get; set; }
    public bool Vents { get; set; }
}

namespace AmogusCompanyMod.Patches {
    class NetworkingPatch {
        public static void SynchronizeImpList() {
            AmogusModBase.mls.LogInfo("Sending impostor list to clients");
            Network.Broadcast("SyncImpList", new Networking() { ImpostorList = AmogusModBase.impostorsIDs, Vents = AmogusModBase.ConfigVents.Value });
            //Also synchronize local config
            if (AmogusModBase.ConfigVents.Value) {
                VentsPatch.SussifyAll();
            }
        }
    }

    class ConsoleCommands {
        public static void RegisterConsoleCommands() {
            AmogusModBase.mls.LogInfo("Registering console commands");
            LC_API.ClientAPI.CommandHandler.RegisterCommand("amogus", (string[] args) => {
                if (CheckConsoleCommand()) {
                    if (int.TryParse(args[0], out int number) && number >= 0 && number <= Player.ActiveList.Count) {
                        var message = "Number of impostors in the game changed to " + args[0];
                        AmogusModBase.mls.LogInfo(message);
                        AmogusModBase.ConfigImpostorCount.Value = number;
                        Player.LocalPlayer.QueueTip("Success", message, 1f, 0, false);
                    } else {
                        AmogusModBase.mls.LogInfo("Invalid argument");
                        Player.LocalPlayer.QueueTip("Error", "Invalid argument - accepted arguments: [0-100%]", 3f, default, true);
                    }
                }
            });

            LC_API.ClientAPI.CommandHandler.RegisterCommand("susdebug", (string[] args) => {
                AmogusModBase.DebugMode = true;
                AmogusModBase.mls.LogInfo("Debug mode enabled");
                Player.LocalPlayer.QueueTip("Succes", "Debug mode enabled", 1f, 0, false);
            });

            LC_API.ClientAPI.CommandHandler.RegisterCommand("susvent", (string[] args) => {
                if (CheckConsoleCommand()) {
                    if (args[0] == "yes" || args[0] == "true" || args[0] == "1") {
                        AmogusModBase.mls.LogInfo("Impostors Vents changed to true");
                        AmogusModBase.ConfigVents.Value = true;
                        Player.LocalPlayer.QueueTip("Succes", "Impostors Vents changed succesfuly to true ", 1f, 0, false);
                    } else if (args[0] == "no" || args[0] == "false" || args[0] == "0") {
                        AmogusModBase.mls.LogInfo("Impostors Vents changed to false");
                        AmogusModBase.ConfigVents.Value = false;
                        Player.LocalPlayer.QueueTip("Succes", "Impostors Vents changed succesfuly to false ", 1f, 0, false);
                    } else {
                        AmogusModBase.mls.LogInfo("Invalid argument");
                        Player.LocalPlayer.QueueTip("Error", "Invalid argument - accepted arguments: yes , no", 3f, default, true);
                    }
                }
            });

            LC_API.ClientAPI.CommandHandler.RegisterCommand("sushelp", (string[] args) => {
                Player.LocalPlayer.QueueTip("HELPING", "/amogus [number of all players] | /susvent [yes,no] ", 5f, 0, false);
            });

        }
        public static bool CheckConsoleCommand() {
            if (LC_API.GameInterfaceAPI.GameState.ShipState == LC_API.Data.ShipState.InOrbit) {
                if (Player.LocalPlayer.IsHost) {
                    return true;
                } else {
                    AmogusModBase.mls.LogInfo("You are not the host");
                    Player.LocalPlayer.QueueTip("Error", "You are not the host", 3f, default, true);
                }
            } else {
                AmogusModBase.mls.LogInfo("Can't change impostors config while not in orbit");
                Player.LocalPlayer.QueueTip("Error", "Can't change impostors config while not in orbit", 3f, default, true);
            }
            return false;
        }
    }
}
