using Microsoft.Xna.Framework;
using POpusCodec;
using POpusCodec.Enums;
using System;
using Terraria;
using Terraria.ModLoader;
using TerraVoice.Native;

namespace TerraVoice.Core;

[Autoload(Side = ModSide.Client)]
internal sealed class VoiceProcessingSystem : ModSystem
{
    public bool NoiseSuppression { get; set; }

    public bool TestMode { get; set; }

    public bool PushToTalk { get; set; }

    public float Amplification { get; set; }

    public float Threshold { get; set; }

    public event Action<short[]> OnTestBufferReceived;

    // https://wiki.xiph.org/Opus_Recommended_Settings recommends 24Kb/s for fullband VoIP.
    private const int VoIPBitrate = 24_000;

    private OpusEncoder encoder;

    private VoiceOutputSystem outputSystem;

    private short[] emptyBuffer;

    public override void PostSetupContent()
    {
        rnnoise.rnnoise_create();

        encoder = new(SamplingRate.Sampling48000, Channels.Mono, OpusApplicationType.Voip, Delay.Delay20ms)
        {
            Bitrate = VoIPBitrate
        };

        outputSystem = ModContent.GetInstance<VoiceOutputSystem>();

        emptyBuffer = new short[(int)(VoiceInputSystem.SampleRate * (VoiceInputSystem.MicrophoneInputDurationMs / 1000f))];
    }

    public override void Unload()
    {
        rnnoise.rnnoise_destroy();
    }

    public override void PreSaveAndQuit()
    {
        NoiseSuppression = false;
        TestMode = false;
    }

    public void SubmitBuffer(short[] buffer)
    {
        if (NoiseSuppression)
            rnnoise.rnnoise_process_frame(buffer);

        float sum = 0;

        for (int i = 0; i < buffer.Length; i++)
        {
            float sample = buffer[i];
            sum += sample * sample;

            buffer[i] = (short)MathHelper.Clamp(buffer[i] * Amplification, short.MinValue, short.MaxValue);
        }

        // Formula for the decibel level of a sample chunk is 20 * log10(RMS), where RMS is the root-mean-square value of the sample.
        float rms = MathF.Sqrt(sum / buffer.Length);
        float dB = 20 * MathF.Log10(rms);

        // Remove input if on open mic mode and input is too quiet.
        if (!PushToTalk && dB < Threshold)
        {
            // Make sure visualizer doesn't freeze.
            OnTestBufferReceived?.Invoke(emptyBuffer);
            return;
        }

        byte[] encoded = encoder.Encode(buffer);

        // When testing, submit the encoded buffer as though it was recieved as a packet.
        // This is more accurate for testing purposes as it factors in the encode/decode other players will hear.
        if (TestMode)
        {
            outputSystem.RecieveBuffer(encoded, Main.myPlayer);
            OnTestBufferReceived?.Invoke(buffer);
        }

        // Don't push input if PTT is enabled and the key isn't pressed.
        if (PushToTalk && !TerraVoice.PushToTalkActivated)
        {
            return;
        }

        TerraVoice.Instance.PushVoiceBuffer(encoded);
    }
}
