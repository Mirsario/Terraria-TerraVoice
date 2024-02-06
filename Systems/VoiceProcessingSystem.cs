using POpusCodec;
using POpusCodec.Enums;
using Terraria;
using Terraria.ModLoader;
using TerraVoice.Native;

namespace TerraVoice.Core;

[Autoload(Side = ModSide.Client)]
internal class VoiceProcessingSystem : ModSystem
{
    public bool NoiseSuppression { get; set; }

    public bool TestMode { get; set; }

    private OpusEncoder encoder;

    private VoiceOutputSystem outputSystem;

    public override void PostSetupContent()
    {
        rnnoise.rnnoise_create();

        encoder = new(SamplingRate.Sampling48000, Channels.Mono);

        outputSystem = ModContent.GetInstance<VoiceOutputSystem>();
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

        byte[] encoded = encoder.Encode(buffer);

        // When testing, submit the encoded buffer as though it was recieved as a packet.
        // This is more accurate for testing purposes as it factors in the encode/decode other players will hear.
        if (TestMode)
            outputSystem.RecieveBuffer(encoded, Main.myPlayer);

        TerraVoice.Instance.PushVoiceBuffer(encoded);
    }
}
