using System;
using NAudio.Wave.SampleProviders;

namespace TerraVoice.Core;

public class SinPanStrategyWithVolume  : IPanStrategy
{
    private const float HalfPi = (float)Math.PI / 2;
    public float Volume;

    /// <summary>
    /// Gets the left and right channel multipliers for this pan value
    /// </summary>
    /// <param name="pan">Pan value, between -1 and 1</param>
    /// <returns>Left and right multipliers</returns>
    public StereoSamplePair GetMultipliers(float pan)
    {
        // -1..+1  -> 1..0
        float normPan = (-pan + 1) / 2;
        float centerFactor = (float)Math.Sin(0.5f * HalfPi);
        float leftChannel = (float)Math.Sin(normPan * HalfPi) * Volume / centerFactor;
        float rightChannel = (float)Math.Cos(normPan * HalfPi) * Volume / centerFactor;
        // Main.NewText(pan + ": " + leftChannel + "," + rightChannel);
        return new StereoSamplePair { Left = leftChannel, Right = rightChannel };
    }
}