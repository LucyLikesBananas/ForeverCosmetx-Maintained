using System.Reflection;
using BepInEx;
using GorillaNetworking;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ForeverCosmetx
{
    [BepInPlugin("com.lucylikesbananas.forevercosmetx", "ForeverCosmetx", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;
        public static bool Hiding;

        private readonly Harmony _harmony = new Harmony("com.lucylikesbananas.forevercosmetx");
        private bool _showMenu;
        private Rect _window = new Rect(30, 30, 200, 100);

        private void Awake()
        {
            Instance = this;
            _harmony.PatchAll();
            Logger.LogInfo("ForeverCosmetx ready - Originally made by iiDK/goldentrophy, maintained by LucyLikesBananas. Press X for the menu.");
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame)
                _showMenu = !_showMenu;
        }

        private void OnGUI()
        {
            if (!_showMenu)
                return;

            _window = GUILayout.Window(0x63534d58, _window, DrawMenu, "CosmetX");
        }

        private void DrawMenu(int id)
        {
            GUILayout.Space(4);

            if (GUILayout.Button(Hiding ? "Show cosmetics" : "Hide cosmetics", GUILayout.Height(30)))
                Hiding = !Hiding;

            GUILayout.Space(2);
            GUILayout.Label(Hiding ? "Status: hidden" : "Status: visible");

            GUI.DragWindow();
        }

        public void UnlockAll()
        {
            var controller = CosmeticsController.instance;
            var unlock = typeof(CosmeticsController)
                .GetMethod("UnlockItem", BindingFlags.NonPublic | BindingFlags.Instance);
            if (controller == null || unlock == null)
                return;

            foreach (var item in controller.allCosmetics)
            {
                if (controller.concatStringCosmeticsAllowed.Contains(item.itemName))
                    continue;

                try { unlock.Invoke(controller, new object[] { item.itemName, false }); }
                catch { /* item not unlockable, skip */ }
            }

            controller.OnCosmeticsUpdated.Invoke();
        }
    }
}
