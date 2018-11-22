using System;

[Serializable]
public class ModSettings
{
    public float decayRateExponent;

    public ModSettings()
    {
        decayRateExponent = 1.9f;
    }

    public ModSettings(ModSettings clone)
    {
        decayRateExponent = clone.decayRateExponent;
    }
}