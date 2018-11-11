using System;
using Harmony;

namespace MoreSailsMoreSpeed.Patches
{
	// Token: 0x02000006 RID: 6
	[HarmonyPatch(typeof(Raft))]
	[HarmonyPatch("Start")]
	internal class RaftMaxVelocityPatch
	{
		// Token: 0x06000020 RID: 32 RVA: 0x00002A3A File Offset: 0x00000C3A
		private static void Postfix(Raft __instance)
		{
			__instance.maxVelocity = 100f;
		}
	}
}
