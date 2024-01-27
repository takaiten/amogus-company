using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using LC_API.GameInterfaceAPI.Features;

namespace AmogusCompanyMod.Patches {
    [HarmonyPatch(typeof(PlayerControllerB))]
    class PlayerControllerBPatch {

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static public void PlayerControllerUpdate() {

            IEnumerator<Player> activePlayers = Player.ActiveList.GetEnumerator();
            while (activePlayers.MoveNext()) {
                if (AmogusModBase.impostorsIDs.Contains(activePlayers.Current.ClientId) && activePlayers.Current.IsControlled &&
                    (AmogusModBase.impostorsIDs.Contains(Player.LocalPlayer.ClientId))) {
                    activePlayers.Current.PlayerController.usernameBillboardText.canvasRenderer.SetColor(UnityEngine.Color.red);
                    activePlayers.Current.PlayerController.usernameAlpha.alpha = 1f;
                } else {
                    activePlayers.Current.PlayerController.usernameBillboardText.canvasRenderer.SetColor(UnityEngine.Color.white);
                }
            }

            try {
                if (AmogusModBase.impostorsIDs.Contains(Player.LocalPlayer.ClientId)) {
                    Player.LocalPlayer.PlayerController.sprintMeter = 1f;
                    Player.LocalPlayer.PlayerController.nightVision.enabled = true;
                }

            } catch {
                AmogusModBase.mls.LogInfo("Failed to get Player.LocalPlayer.ClientId");
            }

            //get impostor status debug
            if (BepInEx.UnityInput.Current.GetKeyDown("F5")) {
                AmogusModBase.mls.LogInfo("F5 pressed");
                if (AmogusModBase.DebugMode) {
                    int a = 111111111;
                    int b = 4;
                    if (!AmogusModBase.impostorsIDs.Contains(Player.LocalPlayer.ClientId)) {
                        StartOfRoundPatch.ImpostorStartGame(ref a, ref b);
                    }
                }
            }

            //remove impostor status debug
            if (BepInEx.UnityInput.Current.GetKeyDown("F6")) {
                AmogusModBase.mls.LogInfo("F6 pressed");
                if (AmogusModBase.DebugMode) {
                    OtherFunctions.RemoveImposter();
                }
            }

            //give impostor item debug shotgun
            if (BepInEx.UnityInput.Current.GetKeyDown("F7")) {
                AmogusModBase.mls.LogInfo("F7 pressed");
                if (AmogusModBase.DebugMode) {
                    OtherFunctions.GetImpostorStartingItem(7, player: Player.LocalPlayer);
                }
            }


            if (BepInEx.UnityInput.Current.GetKeyDown("F8")) {
                AmogusModBase.mls.LogInfo("F8 pressed");
                if (AmogusModBase.DebugMode) {
                    Player.LocalPlayer.PlayerController.movementSpeed = 30f;
                    Player.LocalPlayer.PlayerController.climbSpeed = 100f;
                }
            }

            if (BepInEx.UnityInput.Current.GetKeyDown("F9")) {
                AmogusModBase.mls.LogInfo("F9 pressed");
                if (AmogusModBase.DebugMode) {
                }
            }

        }

        [HarmonyPatch("KillPlayerClientRpc")]
        [HarmonyPostfix]
        static public void KillPlayerClientRpcPatch() {
            OtherFunctions.CheckForImpostorVictory();
        }

        [HarmonyPatch("KillPlayerServerRpc")]
        [HarmonyPostfix]
        static public void KillPlayerServerRpcPatch() {
            OtherFunctions.CheckForImpostorVictory();
        }
    }
}
