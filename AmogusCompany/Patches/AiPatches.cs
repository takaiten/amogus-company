using GameNetcodeStuff;
using HarmonyLib;

namespace AmogusCompanyMod.Patches {
    [HarmonyPatch(typeof(EnemyAI))]
    class EnemyAiPatch {
        //Larva get stuck when it fall on impostor (at least on single player)

        [HarmonyPatch(nameof(EnemyAI.PlayerIsTargetable))]
        [HarmonyPostfix]
        static public void MonstersDontTarget(ref PlayerControllerB playerScript, ref bool __result) {
            if (AmogusModBase.impostorsIDs.Contains(playerScript.actualClientId)) {
                __result = false;
                AmogusModBase.mls.LogInfo("Player " + playerScript.actualClientId + " is impostor and is not targetable");
            }
        }

        [HarmonyPatch((typeof(EnemyAI)), (nameof(EnemyAI.MeetsStandardPlayerCollisionConditions)))]
        [HarmonyPostfix]
        static public void MeetsStandardPlayerCollisionConditions(ref PlayerControllerB __result) {
            try {
                if (AmogusModBase.impostorsIDs.Contains(__result.actualClientId)) {
                    AmogusModBase.mls.LogInfo("Player " + __result.actualClientId + " is impostor and shouldnt colide with mobs");
                    __result = null;
                }
            } catch {

            }

        }

        [HarmonyPatch(nameof(EnemyAI.CheckLineOfSightForPlayer))]
        [HarmonyPostfix]
        static public void CheckLineOfSightForPlayerOverwrite(ref PlayerControllerB __result) {
            try {
                if (AmogusModBase.impostorsIDs.Contains(__result.actualClientId)) {
                    AmogusModBase.mls.LogInfo("Player " + __result.actualClientId + " is impostor and should not be in line of sight check");
                    __result = null;
                    //We might have a problem with situation when impostor is first in check line of sight, mob might get stuck then.
                }
            } catch {

            }

        }

    }

}