using Harmony;
using MoreSailsMoreSpeed.Patches;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace MoreSailsMoreSpeed
{
    [ModTitle("MoreSailsMoreSpeed")]
    [ModDescription("More sails can make your raft faster.")]
    [ModAuthor("Akitake")]
    [ModIconUrl("https://i.imgur.com/eaGHF1J.png")]
    [ModWallpaperUrl("https://i.imgur.com/D7OvpcL.png")]
    [ModVersion("1.0.7")]
    [RaftVersion("Update 8 (3288722)")]
    public class MoreSailsMoreSpeedMod : Mod
    {
        public HarmonyInstance harmony;
        public readonly string harmonyID = "com.github.akitakekun.moresailsmorespeed";

        private Settings settings;
        private string settingsPath;

        public void Start()
        {
            CLIU.CONSOLE_PREFIX = CLIU.Cyan("[") + "MoreSailsMoreSpeed" + CLIU.Cyan("] ");
            this.harmony = HarmonyInstance.Create(this.harmonyID);
            this.harmony.PatchAll(Assembly.GetExecutingAssembly());
            this.settingsPath = Directory.GetCurrentDirectory() + "\\mods\\MoreSailsMoreSpeed.json";
            this.settings = this.LoadSettings();
            RaftFixedUpdatePatch.rate = this.settings.decayRateExponent;
            RConsole.registerCommand(typeof(Mod), "Lower all sails", "sailsOpen", new Action(MoreSailsMoreSpeedMod.SailsOpen));
            RConsole.registerCommand(typeof(Mod), "Raise all sails", "sailsClose", new Action(MoreSailsMoreSpeedMod.SailsClose));
            RConsole.registerCommand(typeof(Mod), "The exponent x for i/i^x (default: 1.9; constrained to 1 <= x <= 5)", "sailsDecay", new Action(this.SailsDecay));
            CLIU.Echo("loaded!");
        }

        public void OnModUnload()
        {
            CLIU.Echo("unloaded!");
            this.harmony.UnpatchAll(this.harmonyID);
            UnityEngine.Object.Destroy(base.gameObject);
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
            Settings settings = new Settings();
            float num;
            if (!CLIU.InterpretFloatFromLastCommand(out num, 1f, 5f, settings.decayRateExponent))
            {
                return;
            }
            RaftFixedUpdatePatch.rate = num;
            this.settings.decayRateExponent = num;
            this.SaveSettings();
            CLIU.Echo("sailsDecay rate is now set to " + CLIU.Blue(num.ToString()));
        }

        private Settings LoadSettings()
        {
            Settings result = new Settings();
            if (File.Exists(this.settingsPath) && new FileInfo(this.settingsPath).Length > 0L)
            {
                result = JsonUtility.FromJson<Settings>(File.ReadAllText(this.settingsPath));
            }
            return result;
        }

        private void SaveSettings()
        {
            try
            {
                string contents = JsonUtility.ToJson(this.settings);
                File.WriteAllText(this.settingsPath, contents);
            }
            catch
            {
                CLIU.Echo("Settings were unable to be saved to file " + this.settingsPath);
            }
        }
    }
}
