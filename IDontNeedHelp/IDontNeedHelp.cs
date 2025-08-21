using FrooxEngine;
using HarmonyLib;
using ResoniteModLoader;
using System;
using System.Reflection;

namespace IDontNeedHelp;

public class IDontNeedHelp : ResoniteMod
{
    internal const string VERSION_CONSTANT = "1.0.0";
    public override string Name => "IDontNeedHelp";
    public override string Author => "nalathethird";
    public override string Version => VERSION_CONSTANT;
    public override string Link => "https://github.com/nalathethird/Fck-HelpSection/";

    public override void OnEngineInit()
    {
        Harmony harmony = new Harmony("com.nalathethird.IDontNeedHelp");
        harmony.PatchAll();
        Msg("IDontNeedHelp mod initialized - Help tab will be hidden.");
    }

    // This patch targets the AddHelpScreen method in UserspaceScreensManager to prevent it from adding the Help screen
    [HarmonyPatch]
    class UserspaceScreensManager_AddHelpScreen_Patch
    {
        static MethodBase TargetMethod()
        {
            return typeof(UserspaceScreensManager).GetMethod("AddHelpScreen", 
                BindingFlags.NonPublic | BindingFlags.Instance);
        }

        static bool Prefix()
        {
            IDontNeedHelp.Msg("Prevented Help tab from being added to RadiantDash");
            return false;
        }
    }

    [HarmonyPatch(typeof(RadiantDash), "UpdateScreens")]
    class RadiantDash_UpdateScreens_Patch
    {
        static void Postfix(RadiantDash __instance)
        {
            try
            {
                foreach (RadiantDashScreen screen in __instance.Screens)
                {
                    if (screen != null && screen.Label.Value == "Help")
                    {
                        screen.ScreenEnabled.Value = false;
                        screen.ScreenRoot.ActiveSelf = false;
                    }
                }
            }
            catch (Exception e)
            {
                IDontNeedHelp.Msg($"Error in Help tab removal: {e.Message}");
            }
        }
    }

    [HarmonyPatch(typeof(RadiantDash), "OnCommonUpdate")]
    class RadiantDash_OnCommonUpdate_Patch
    {
        static void Postfix(RadiantDash __instance)
        {
            try
            {
                if (__instance.CurrentScreen.Target != null && 
                    __instance.CurrentScreen.Target.Label.Value == "Help")
                {
                    foreach (RadiantDashScreen screen in __instance.Screens)
                    {
                        if (screen != null && 
                            screen.Label.Value != "Help" && 
                            screen.ScreenEnabled.Value)
                        {
                            __instance.CurrentScreen.Target = screen;
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                IDontNeedHelp.Msg($"Error in current screen check: {e.Message}");
            }
        }
    }
}
