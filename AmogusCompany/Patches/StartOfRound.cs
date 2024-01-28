using HarmonyLib;
using System.Linq;
using LC_API.GameInterfaceAPI.Features;
using LC_API.Networking;
using LCShrinkRay.comp;
using AmogusCompanyMod.Helpers;
using UnityEngine;

namespace AmogusCompanyMod.Patches {

    [HarmonyPatch(typeof(StartOfRound))]
    class StartOfRoundPatch {
        [HarmonyPatch("OnShipLandedMiscEvents")]
        [HarmonyPostfix]
        public static void ImpostorStartGame(ref int ___currentLevelID) {
            if (___currentLevelID != 3 && Player.LocalPlayer.IsHost) {
                var players = Player.ActiveList.Select(playerX => playerX.ClientId).ToArray();
                // Find N random client ids
                var impostorClientIds = RandomSelection.SelectRandomElements(players, AmogusModBase.ConfigImpostorCount.Value);

                // And set them as impostors
                AmogusModBase.impostorsIDs.Clear();
                AmogusModBase.impostorsIDs.AddRange(impostorClientIds);
                AmogusModBase.lastKillTime = Time.time;

                NetworkingPatch.SynchronizeImpList();
                ImpostorLever();
            }
        }

        [HarmonyPatch("ShipHasLeft")]
        [HarmonyPrefix]
        static public void ShipHasLeftPatch() {
            OtherFunctions.RemoveImposter();
            VentsPatch.unsussifyAll();
        }

        [NetworkMessage("SyncImpList")]
        public static void SyncHandler(ulong sender, Networking message) {
            AmogusModBase.mls.LogInfo("Recived impostors list from host");
            AmogusModBase.impostorsIDs = message.ImpostorList;
            if (AmogusModBase.impostorsIDs.Contains(Player.LocalPlayer.ClientId)) {
                HUDManager.Instance.DisplayTip("Alert", "You Are The Impostor!", true, false, "");
                Player.LocalPlayer.PlayerController.nightVision.intensity = 3000;
                Player.LocalPlayer.PlayerController.nightVision.range = 5000;
            }
            if (message.Vents) {
                VentsPatch.SussifyAll();
            }
            ImpostorLever();
        }

        public static void ImpostorLever() {
            if (AmogusModBase.impostorsIDs.Contains(Player.LocalPlayer.ClientId)) {
                InteractTrigger triggerScript = UnityEngine.Object.FindObjectOfType<StartMatchLever>().triggerScript;
                triggerScript.interactable = false;
                triggerScript.disabledHoverTip = "Impostor can't start the ship";
            }
        }
    }
}
