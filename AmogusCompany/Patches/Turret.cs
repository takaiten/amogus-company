using GameNetcodeStuff;
using HarmonyLib;

namespace AmogusCompanyMod.Patches {
    [HarmonyPatch(typeof(Turret))]
    class TurretPach {
        [HarmonyPatch("CheckForPlayersInLineOfSight")]
        [HarmonyPostfix]
        static public void ExcludeImposterFromLineOfSight(ref PlayerControllerB __result) {
            try {
                if (AmogusModBase.impostorsIDs.Contains(__result.actualClientId)) {
                    AmogusModBase.mls.LogInfo("Player " + __result.actualClientId + " is impostor and is not targetable by turret");
                    __result = null;
                }
            } catch {

            }

        }
    }
}
