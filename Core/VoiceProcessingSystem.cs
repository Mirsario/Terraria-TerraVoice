using Microsoft.Xna.Framework.Input;
using POpusCodec;
using POpusCodec.Enums;
using System;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.ModLoader;
using TerraVoice.Native;

namespace TerraVoice.Core;

[Autoload(Side = ModSide.Client)]
internal class VoiceProcessingSystem : ModSystem
{
    private bool NoiseSuppression { get; set; }

    private bool TestMode { get; set; }

    private OpusEncoder encoder;

    private VoiceOutputSystem outputSystem;

    public override void PostSetupContent()
    {
        // TODO: package rnnoise for multiple platforms. Currently only supports windows x64.
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            rnnoise.rnnoise_create();

        encoder = new(SamplingRate.Sampling48000, Channels.Mono);

        outputSystem = ModContent.GetInstance<VoiceOutputSystem>();
    }

    public override void Unload()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            rnnoise.rnnoise_destroy();
    }

    public override void PreSaveAndQuit()
    {
        NoiseSuppression = false;
        TestMode = false;
    }

    public override void PostUpdateInput()
    {
        if (Main.keyState.IsKeyDown(Keys.N) && Main.oldKeyState.IsKeyUp(Keys.N))
        {
            NoiseSuppression = !NoiseSuppression;
            Main.NewText("NS: " + NoiseSuppression);
        }

        if (Main.keyState.IsKeyDown(Keys.T) && Main.oldKeyState.IsKeyUp(Keys.T))
        {
            TestMode = !TestMode;
            Main.NewText("Test: " + TestMode);
        }
    }

    public void SubmitBuffer(byte[] buffer)
    {
        if (NoiseSuppression && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            rnnoise.rnnoise_process_frame(buffer);

        Span<short> pcmBuffer = MemoryMarshal.Cast<byte, short>(buffer);

        byte[] encoded = encoder.Encode(pcmBuffer);

        // When testing, submit the encoded buffer as though it was recieved as a packet.
        // This is more accurate for testing purposes as it factors in the encode/decode other players will hear.
        if (TestMode)
            outputSystem.RecieveBuffer(encoded, Main.myPlayer);

        TerraVoice.Instance.PushVoiceBuffer(encoded);
    }
}
