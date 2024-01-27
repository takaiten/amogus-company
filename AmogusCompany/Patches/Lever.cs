using HarmonyLib;
using LC_API.GameInterfaceAPI.Features;
using AmogusCompanyMod;

namespace TestMod.Patches {

    [HarmonyPatch(typeof(StartMatchLever))]
    internal class LeverPatch {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void ImpLever() {
            try {
                if (AmogusModBase.impostorsIDs.Contains(Player.LocalPlayer.ClientId)) {
                    InteractTrigger triggerScript = UnityEngine.Object.FindObjectOfType<StartMatchLever>().triggerScript;
                    triggerScript.interactable = false;
                    triggerScript.disabledHoverTip = "Impostor can't start the ship";
                }
            } catch {
                AmogusModBase.mls.LogInfo("Error in LeverPatch");
            }


        }
    }
}
