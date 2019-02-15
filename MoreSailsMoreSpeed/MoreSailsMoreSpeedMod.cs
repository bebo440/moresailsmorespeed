using Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

[ModTitle("MoreSailsMoreSpeed")]
[ModDescription("More sails can make your raft faster.")]
[ModAuthor("Akitake")]
[ModIconUrl("https://i.imgur.com/eaGHF1J.png")]
[ModWallpaperUrl("https://i.imgur.com/D7OvpcL.png")]
[ModVersion("1.2.0")]
[RaftVersion("Update 9 (3556813)")]
public class MoreSailsMoreSpeedMod : Mod
{
    public HarmonyInstance harmony;
    public readonly string harmonyID = "com.github.akitakekun.moresailsmorespeed";

    private ModSettings modSettings;
    private string settingsPath;

    public void Start()
    {
        CLIU.CONSOLE_PREFIX = CLIU.Cyan("[") + "MoreSailsMoreSpeed" + CLIU.Cyan("] ");
        harmony = HarmonyInstance.Create(harmonyID);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        settingsPath = Directory.GetCurrentDirectory() + "\\mods\\MoreSailsMoreSpeed.json";
        modSettings = LoadSettings();
        RaftFixedUpdatePatch.rate = modSettings.decayRateExponent;
        //RConsole.registerCommand(typeof(MoreSailsMoreSpeedMod), "Lower all sails", "sailsOpen", SailsOpen);
        //RConsole.registerCommand(typeof(MoreSailsMoreSpeedMod), "Raise all sails", "sailsClose", SailsClose);
        //RConsole.registerCommand(typeof(MoreSailsMoreSpeedMod), "The exponent x for i/i^x (default: 1.9; constrained to 1 <= x <= 5)", "sailsDecay", SailsDecay);
        RConsole.registerCommand("sailsOpen", "Lower all sails", "sailsOpen", SailsOpen);
        RConsole.registerCommand("sailsClose", "Raise all sails", "sailsClose", SailsClose);
        RConsole.registerCommand("sailsDecay", "The exponent x for i/i^x (default: 1.9; constrained to 1 <= x <= 5)", "sailsDecay", SailsDecay);
        CLIU.Echo("loaded!");
    }

    public void OnModUnload()
    {
        CLIU.Echo("unloaded!");
        RConsole.unregisterCommand("sailsOpen");
        RConsole.unregisterCommand("sailsClose");
        RConsole.unregisterCommand("sailsDecay");
        harmony.UnpatchAll(harmonyID);
        Destroy(gameObject);
    }

    public static void SailsRotate(float axis)
    {
        Semih_Network value = ComponentManager<Semih_Network>.Value;
        List<Sail> allSails = Sail.AllSails;
        Type[] array = new Type[]
        {
                typeof(float)
        };
        object[] parameters = new object[]
        {
                axis
        };
        foreach (Sail sail in allSails)
        {
            if (Semih_Network.IsHost)
            {
                AccessTools.Method("Sail:Rotate", array, null).Invoke(sail, parameters);
            }
            else
            {
                Message message = new Message_Sail_Rotate(Messages.Sail_Rotate, sail, axis);
                value.SendP2P(value.HostID, message, Steamworks.EP2PSend.k_EP2PSendReliable, NetworkChannel.Channel_Game);
            }
        }
    }

    public static void SailsRotate(float axis, Sail __instance)
    {
        Semih_Network value = ComponentManager<Semih_Network>.Value;
        List<Sail> allSails = Sail.AllSails;
        Type[] array = new Type[]
        {
                typeof(float)
        };
        object[] parameters = new object[]
        {
                axis
        };

        if (Semih_Network.IsHost)
        {
            AccessTools.Method("Sail:Rotate", array, null).Invoke(__instance, parameters);
        }
        else
        {
            Message message = new Message_Sail_Rotate(Messages.Sail_Rotate, __instance, axis);
            value.SendP2P(value.HostID, message, Steamworks.EP2PSend.k_EP2PSendReliable, NetworkChannel.Channel_Game);
        }

        foreach (Sail sail in allSails)
        {
            if (__instance != sail)
            {
                float _axis = (__instance.LocalRotation - sail.LocalRotation);
                object[] _parameters = new object[]
                {
                        _axis
                };

                if (Semih_Network.IsHost)
                {
                    AccessTools.Method("Sail:Rotate", array, null).Invoke(sail, _parameters);
                }
                else
                {
                    Message message = new Message_Sail_Rotate(Messages.Sail_Rotate, sail, _axis);
                    value.SendP2P(value.HostID, message, Steamworks.EP2PSend.k_EP2PSendReliable, NetworkChannel.Channel_Game);
                }
            }
        }
    }

    public static void SailsOpen()
    {
        Semih_Network value = ComponentManager<Semih_Network>.Value;
        List<Sail> allSails = Sail.AllSails;
        for (int i = 0; i < allSails.Count; i++)
        {
            Sail sail = allSails[i];
            Message_NetworkBehaviour message_NetworkBehaviour = new Message_NetworkBehaviour((!sail.open) ? Messages.Sail_Open : Messages.Sail_Close, sail);
            if (Semih_Network.IsHost)
            {
                sail.Open();
                value.RPC(message_NetworkBehaviour, Target.All, Steamworks.EP2PSend.k_EP2PSendReliableWithBuffering, 0);
            }
            else
            {
                value.SendP2P(value.HostID, message_NetworkBehaviour, Steamworks.EP2PSend.k_EP2PSendReliableWithBuffering, 0);
            }
        }
    }

    public static void SailsClose()
    {
        Semih_Network value = ComponentManager<Semih_Network>.Value;
        List<Sail> allSails = Sail.AllSails;
        object[] parameters = new object[0];
        for (int i = 0; i < allSails.Count; i++)
        {
            Sail sail = allSails[i];
            Message_NetworkBehaviour message_NetworkBehaviour = new Message_NetworkBehaviour((!sail.open) ? Messages.Sail_Open : Messages.Sail_Close, sail);
            if (Semih_Network.IsHost)
            {
                AccessTools.Method("Sail:Close", null, null).Invoke(sail, parameters);
                value.RPC(message_NetworkBehaviour, Target.All, Steamworks.EP2PSend.k_EP2PSendReliableWithBuffering, 0);
            }
            else
            {
                value.SendP2P(value.HostID, message_NetworkBehaviour, Steamworks.EP2PSend.k_EP2PSendReliableWithBuffering, 0);
            }
        }
    }

    public void SailsDecay()
    {
        ModSettings settings = new ModSettings();
        float num;
        if (!CLIU.InterpretFloatFromLastCommand(out num, 1f, 5f, settings.decayRateExponent))
        {
            return;
        }
        RaftFixedUpdatePatch.rate = num;
        settings.decayRateExponent = num;
        SaveSettings();
        CLIU.Echo("sailsDecay rate is now set to " + CLIU.Blue(num.ToString()));
    }

    private ModSettings LoadSettings()
    {
        ModSettings result = new ModSettings();
        if (File.Exists(settingsPath) && new FileInfo(settingsPath).Length > 0L)
        {
            result = JsonUtility.FromJson<ModSettings>(File.ReadAllText(settingsPath));
        }
        return result;
    }

    private void SaveSettings()
    {
        try
        {
            string contents = JsonUtility.ToJson(modSettings);
            File.WriteAllText(settingsPath, contents);
        }
        catch
        {
            CLIU.Echo("Settings were unable to be saved to file " + settingsPath);
        }
    }
}
