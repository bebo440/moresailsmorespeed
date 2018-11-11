﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Harmony;
using MoreSailsMoreSpeed.Patches;
using UnityEngine;

namespace MoreSailsMoreSpeed
{
	// Token: 0x02000003 RID: 3
	[ModTitle("MoreSailsMoreSpeed")]
	[ModDescription("More sails can make your raft faster.")]
	[ModAuthor("RumRunner, Patched by Akitake")]
    [ModIconUrl("https://i.imgur.com/eaGHF1J.png")]
    [ModWallpaperUrl("https://i.imgur.com/D7OvpcL.png")]
    [ModVersion("1.25")]
    [RaftVersion("Update 8 (3288722)")]
    public class MoreSailsMoreSpeedMod : Mod
	{
		// Token: 0x06000012 RID: 18 RVA: 0x000024D4 File Offset: 0x000006D4
		private void Start()
		{
			CLIU.CONSOLE_PREFIX = CLIU.Blue("[") + "MoreSailsMoreSpeed" + CLIU.Blue("] ");
			this.harmony = HarmonyInstance.Create(this.harmonyID);
			this.harmony.PatchAll(Assembly.GetExecutingAssembly());
			this.settingsPath = Directory.GetCurrentDirectory() + "\\mods\\MoreSailsMoreSpeed.json";
			this.settings = this.LoadSettings();
			RaftFixedUpdatePatch.rate = this.settings.decayRateExponent;
			RConsole.registerCommand("sailsOpen", "Lower all sails", "sailsOpen", new Action(MoreSailsMoreSpeedMod.SailsOpen));
			RConsole.registerCommand("sailsClose", "Raise all sails", "sailsClose", new Action(MoreSailsMoreSpeedMod.SailsClose));
			RConsole.registerCommand("sailsDecay", "The exponent x for i/i^x (default: 1.9; constrained to 1 <= x <= 5)", "sailsDecay", new Action(this.SailsDecay));
			CLIU.Echo("loaded!");
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000025C0 File Offset: 0x000007C0
		public void OnModUnload()
		{
			RConsole.unregisterCommand("sailsOpen");
			RConsole.unregisterCommand("sailsClose");
			RConsole.unregisterCommand("sailsDecay");
			this.harmony.UnpatchAll(this.harmonyID);
            UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000025FC File Offset: 0x000007FC
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

		// Token: 0x06000015 RID: 21 RVA: 0x00002694 File Offset: 0x00000894
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

		// Token: 0x06000016 RID: 22 RVA: 0x00002708 File Offset: 0x00000908
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

		// Token: 0x06000017 RID: 23 RVA: 0x00002794 File Offset: 0x00000994
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

		// Token: 0x06000018 RID: 24 RVA: 0x000027F4 File Offset: 0x000009F4
		private Settings LoadSettings()
		{
			Settings result = new Settings();
			if (File.Exists(this.settingsPath) && new FileInfo(this.settingsPath).Length > 0L)
			{
				result = JsonUtility.FromJson<Settings>(File.ReadAllText(this.settingsPath));
			}
			return result;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x0000283C File Offset: 0x00000A3C
		private void SaveSettings()
		{
			try
			{
				string contents = JsonUtility.ToJson(this.settings);
				File.WriteAllText(this.settingsPath, contents);
			}
			catch
			{
				RConsole.Log("Settings were unable to be saved to file " + this.settingsPath);
			}
		}

		// Token: 0x04000007 RID: 7
		private readonly string harmonyID = "com.github.akitakekun.moresailsmorespeed";

		// Token: 0x04000008 RID: 8
		private HarmonyInstance harmony;

		// Token: 0x04000009 RID: 9
		private Settings settings;

		// Token: 0x0400000A RID: 10
		private string settingsPath;
	}
}