using Harmony;
using UnityEngine;

namespace MoreSailsMoreSpeed.Patches
{
    [HarmonyPatch(typeof(Sail))]
    [HarmonyPatch("OnMouseOver")]
    internal class SailMouseOverPatch
    {
        private static bool Prefix(Sail __instance, ref CanvasHelper ___canvas, ref Network_Player ___localPlayer)
        {
            if (!MyInput.GetButton("Sprint"))
            {
                return true;
            }
            if (Helper.LocalPlayerIsWithinDistance(__instance.transform.position, Player.UseDistance) && CanvasHelper.ActiveMenu == MenuType.None)
            {
                if (!__instance.open)
                {
                    ___canvas.displayTextManager.ShowText(Helper.GetTerm("Game/Open", false), MyInput.Keybinds["Interact"].MainKey, 0, true);
                }
                else
                {
                    ___canvas.displayTextManager.ShowText(Helper.GetTerm("Game/RotateSmooth2", false), MyInput.Keybinds["Rotate"].MainKey, 0, true);
                }
                if (MyInput.GetButtonDown("Interact"))
                {
                    if (Semih_Network.IsHost)
                    {
                        if (__instance.open)
                        {
                            MoreSailsMoreSpeedMod.SailsClose();
                        }
                        else
                        {
                            MoreSailsMoreSpeedMod.SailsOpen();
                        }
                    }
                }

                if (MyInput.GetButton("Rotate"))
                {
                    if (___localPlayer.PlayerScript.MouseLookIsActive())
                        ___localPlayer.PlayerScript.SetMouseLookScripts(false);
                    MoreSailsMoreSpeedMod.SailsRotate(Input.GetAxis("Mouse X"), __instance);
                }
                else if (MyInput.GetButtonUp("Rotate"))
                {
                    ___localPlayer.PlayerScript.SetMouseLookScripts(true);
                }
                return false;
            }
            else
            {
                ___canvas.displayTextManager.HideDisplayTexts();
            }
            return false;
        }
    }
}
