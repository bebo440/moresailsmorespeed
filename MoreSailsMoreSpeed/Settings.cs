using System;

namespace MoreSailsMoreSpeed
{
    [Serializable]
    public class Settings
    {
        public Settings()
        {
            this.decayRateExponent = 1.9f;
        }

        public Settings(Settings clone)
        {
            this.decayRateExponent = clone.decayRateExponent;
        }

        public float decayRateExponent;
    }
}
