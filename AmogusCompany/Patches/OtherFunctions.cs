using System.Collections.Generic;
using GameNetcodeStuff;
using LC_API.GameInterfaceAPI.Features;
using UnityEngine;

namespace AmogusCompanyMod.Patches {
    class OtherFunctions {
        static public void CheckForImpostorVictory() {
            AmogusModBase.mls.LogInfo("Checking for Impostor Victory");
            int aliveCrewMates = 0;
            IEnumerator<Player> activePlayers = Player.ActiveList.GetEnumerator();
            while (activePlayers.MoveNext()) {
                if (!activePlayers.Current.IsDead && !AmogusModBase.impostorsIDs.Contains(activePlayers.Current.ClientId)) {
                    aliveCrewMates++;
                }

            }
            AmogusModBase.mls.LogInfo("aliveCrewMates is : " + aliveCrewMates);
            if (aliveCrewMates == 0) {
                AmogusModBase.mls.LogInfo("Impostors Won");
                StartOfRound.Instance.ShipLeaveAutomatically();
            }
        }

        static public void OnDiedCheckForImpostorVictory(LC_API.GameInterfaceAPI.Events.EventArgs.Player.DyingEventArgs dyingEventArgs) {
            CheckForImpostorVictory();
        }
        static public void OnLeftCheckForImpostorVictory(LC_API.GameInterfaceAPI.Events.EventArgs.Player.LeftEventArgs leftEventArgs) {
            if (leftEventArgs.Player.IsLocalPlayer) {
                AmogusModBase.mls.LogInfo("Local player has left, Clearing Player.Dictionary");
                Player.Dictionary.Clear();
            }
            CheckForImpostorVictory();
        }

        public static void RemoveImposter() {
            AmogusModBase.impostorsIDs.Clear();
            Player.LocalPlayer.PlayerController.nightVision.intensity = 1000;
            Player.LocalPlayer.PlayerController.nightVision.range = 2000;
            Player.LocalPlayer.PlayerController.nightVision.enabled = false;
            AmogusModBase.mls.LogInfo("Removing Impostors");
        }
    }
}

