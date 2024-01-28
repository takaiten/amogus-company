using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using LC_API.GameInterfaceAPI.Features;
using UnityEngine;
using TMPro;

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

            if (BepInEx.UnityInput.Current.GetKeyDown("F9")) {
                AmogusModBase.DebugMode = true;
                AmogusModBase.mls.LogInfo("Debug Mode Toggle: " + AmogusModBase.DebugMode);
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

        [HarmonyPatch("SetHoverTipAndCurrentInteractTrigger")]
        [HarmonyPostfix]
        static public void TooltipPatch(PlayerControllerB __instance) {
            if (!AmogusModBase.impostorsIDs.Contains(Player.LocalPlayer.ClientId)) {
                return;
            }
            var interactRay = new Ray(__instance.gameplayCamera.transform.position, __instance.gameplayCamera.transform.forward);
            if (!__instance.isFreeCamera && Physics.Raycast(interactRay, out var hit, 5f, 8)) {
                PlayerControllerB playerLookingAt = hit.collider.gameObject.GetComponent<PlayerControllerB>();
                if (playerLookingAt != null && __instance.playerClientId != playerLookingAt.playerClientId) {
                    var ellapsed = Time.time - AmogusModBase.lastKillTime;
                    if (ellapsed >= 60) {
                        __instance.cursorTip.text = "KILL " + playerLookingAt.playerUsername;
                    } else {
                        __instance.cursorTip.text = $"KILL on cooldown: {(int)(60 - ellapsed)}s";
                    }
                }
            }
        }

        [HarmonyPatch("BeginGrabObject")]
        [HarmonyPrefix]
        static public bool GrabObjectPatch(PlayerControllerB __instance) {
            AmogusModBase.mls.LogMessage("Trying to do the sussy");
            // Only impostor can kill
            if (!AmogusModBase.impostorsIDs.Contains(Player.LocalPlayer.ClientId)) {
                return true;
            }
            // Check for cooldown
            var ellapsed = Time.time - AmogusModBase.lastKillTime;
            AmogusModBase.mls.LogMessage($"Ellapsed since last kill: {ellapsed}");
            if (ellapsed < 60) {
                return true;
            }
            // Find the target player and kill
            var interactRay = new Ray(__instance.gameplayCamera.transform.position, __instance.gameplayCamera.transform.forward);
            if (Physics.Raycast(interactRay, out var hit, 5f, 8)) {
                PlayerControllerB playerLookingAt = hit.collider.gameObject.GetComponent<PlayerControllerB>();
                if (playerLookingAt != null && __instance.playerClientId != playerLookingAt.playerClientId) {
                    playerLookingAt.DamagePlayerFromOtherClientServerRpc(100, Vector3.zero, (int)Player.LocalPlayer.ClientId);
                    AmogusModBase.mls.LogMessage("Killing player: " + playerLookingAt.playerUsername);

                    AmogusModBase.lastKillTime = Time.time;
                    return false;
                }
            }
            return true;
        }
    }
}
