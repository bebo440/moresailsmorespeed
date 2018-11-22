using Harmony;

[HarmonyPatch(typeof(Raft))]
[HarmonyPatch("Start")]
class RaftMaxVelocityPatch
{
    static void Postfix(Raft __instance)
    {
        __instance.maxVelocity = 100f;
    }
}

