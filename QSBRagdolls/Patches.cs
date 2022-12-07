using HarmonyLib;
using QSB.Player;
using QSB.RespawnSync;

namespace QSBRagdolls{

    [HarmonyPatch]
    public class Patches{
        [HarmonyPrefix]
        [HarmonyPatch(typeof(RespawnManager), nameof(RespawnManager.OnPlayerDeath))]
        public static void AfterPlayerDeath(PlayerInfo player){
            //We need to create a ragdoll of the player that died here
            //To create the ragdoll we need 
            QSBRagdolls.CreateRagdoll(player);
        }
    }
}