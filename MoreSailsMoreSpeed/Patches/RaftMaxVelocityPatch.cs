using Harmony;

namespace MoreSailsMoreSpeed.Patches
{
	[HarmonyPatch(typeof(Raft))]
	[HarmonyPatch("Start")]
	internal class RaftMaxVelocityPatch
	{
		private static void Postfix(Raft __instance)
		{
			__instance.maxVelocity = 100f;
		}
	}
}
