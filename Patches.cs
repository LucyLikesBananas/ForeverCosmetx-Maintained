using GorillaNetworking;
using GorillaNetworking.Store;
using HarmonyLib;
using UnityEngine;

namespace ForeverCosmetx
{
    [HarmonyPatch(typeof(VRRig), "IsItemAllowed")]
    internal static class AllowEverything
    {
        private static void Postfix(ref bool __result) => __result = true;
    }

    [HarmonyPatch(typeof(CosmeticsController.CosmeticSet), "LoadFromPlayerPreferences")]
    internal static class KeepEquipped
    {
        private static bool Prefix(CosmeticsController.CosmeticSet __instance, CosmeticsController controller)
        {
            for (int i = 0; i < 16; i++)
            {
                var slot = (CosmeticsController.CosmeticSlots)i;
                var saved = PlayerPrefs.GetString(CosmeticsController.CosmeticSet.SlotPlayerPreferenceName(slot), "NOTHING");
                __instance.items[i] = controller.GetItemFromDict(saved);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(StoreUpdater), "Initialize")]
    internal static class UnlockOnStoreLoad
    {
        private static void Postfix() => Plugin.Instance.UnlockAll();
    }

    [HarmonyPatch(typeof(VRRig), "RequestCosmetics")]
    internal static class HideCosmetics
    {
        private static bool Prefix() => !Plugin.Hiding;
    }
}
