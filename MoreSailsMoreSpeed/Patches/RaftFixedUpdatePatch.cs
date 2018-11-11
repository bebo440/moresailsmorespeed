using System;
using System.Collections.Generic;
using FMODUnity;
using Harmony;
using UnityEngine;

namespace MoreSailsMoreSpeed.Patches
{
	[HarmonyPatch(typeof(Raft))]
	[HarmonyPatch("FixedUpdate")]
	internal class RaftFixedUpdatePatch
	{
		private static bool Prefix(Raft __instance, ref Rigidbody ___body, ref float ___speed, ref StudioEventEmitter ___eventEmitter_idle, ref Vector3 ___previousPosition)
		{
			if (!Semih_Network.IsHost)
			{
				return false;
			}
			if (!__instance.IsAnchored && ___speed != 0f)
			{
				__instance.moveDirection = Vector3.forward;
				List<Sail> allSails = Sail.AllSails;
				Vector3 vector = Vector3.zero;
				int i = 0;
				int num = 1;
				while (i < allSails.Count)
				{
					Sail sail = allSails[i];
					if (sail.open)
					{
						vector += sail.GetNormalizedDirection() * (float)((double)num / Math.Pow((double)num, (double)RaftFixedUpdatePatch.rate));
					}
					i++;
					num++;
				}
				if (vector.z < 0f)
				{
					vector.z = (((double)Mathf.Abs(vector.x) <= 0.7) ? -0.8f : (__instance.moveDirection.z = 0f));
				}
				__instance.moveDirection += vector;
				___body.AddForce(__instance.moveDirection * ___speed);
			}
			if (___body.velocity.sqrMagnitude > __instance.maxVelocity)
			{
				___body.velocity = Vector3.ClampMagnitude(___body.velocity, __instance.maxVelocity);
			}
			___eventEmitter_idle.SetParameter("velocity", ___body.velocity.sqrMagnitude / __instance.maxVelocity);
			___previousPosition = ___body.transform.position;
			return false;
		}

		public static float rate = 1.9f;
	}
}
