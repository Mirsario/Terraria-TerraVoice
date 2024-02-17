using Terraria;
using Terraria.ModLoader;
using TerraVoice.IO;
using TerraVoice.Native;

namespace TerraVoice.Core;

[Autoload(Side = ModSide.Client)]
internal sealed class VoiceInputSystem : ModSystem
{
    // The rnnoise library only works with a sampling rate of 48KHz.
    public const int SampleRate = 48_000;

    // Used for convenience as the opus default frame size is 20ms - https://wiki.xiph.org/Opus_Recommended_Settings.
    public const int MicrophoneInputDurationMs = 20;

    private VoiceProcessingSystem processingSystem;

    private ALMono16Microphone microphone;

    private bool recording;

    public override void PostSetupContent()
    {
        UserDataStore data = PersistentDataStoreSystem.GetDataStore<UserDataStore>();

        SwitchAudioDevice(data.Device.Value);

        processingSystem = ModContent.GetInstance<VoiceProcessingSystem>();
    }

    public override void Unload()
    {
        microphone?.Dispose();
        microphone = null;
    }

    public override void PreSaveAndQuit()
    {
        microphone.StopRecording();
        microphone.OnBufferReady -= HandleAudioInputBuffer;

        recording = false;
    }

    public override void PostUpdateEverything()
    {
        UserDataStore data = PersistentDataStoreSystem.GetDataStore<UserDataStore>();

        if (data.MicrophoneEnabled.Value && !recording)
        {
            microphone.OnBufferReady += HandleAudioInputBuffer;
            microphone.StartRecording();

            recording = true;
        }
    }

    private void HandleAudioInputBuffer(short[] buffer)
    {
        if (!Main.gameMenu)
        {
            processingSystem.SubmitBuffer(buffer);
        }
    }

    private void SwitchAudioDevice(string device)
    {
        if (microphone != null)
        {
            microphone.OnBufferReady -= HandleAudioInputBuffer;
            microphone.StopRecording();
            microphone.Dispose();
        }

        microphone = new ALMono16Microphone(device, MicrophoneInputDurationMs, SampleRate);

        if (recording)
        {
            microphone.OnBufferReady += HandleAudioInputBuffer;
            microphone.StartRecording();
        }
    }
}

