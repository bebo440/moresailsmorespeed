using System;

namespace MoreSailsMoreSpeed
{
	// Token: 0x02000004 RID: 4
	[Serializable]
	public class Settings
	{
		// Token: 0x0600001B RID: 27 RVA: 0x0000289F File Offset: 0x00000A9F
		public Settings()
		{
			this.decayRateExponent = 1.9f;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000028B2 File Offset: 0x00000AB2
		public Settings(Settings clone)
		{
			this.decayRateExponent = clone.decayRateExponent;
		}

		// Token: 0x0400000B RID: 11
		public float decayRateExponent;
	}
}
