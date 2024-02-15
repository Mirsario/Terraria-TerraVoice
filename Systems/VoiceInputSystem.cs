using System.Collections.Generic;
using Terraria.ModLoader;
using TerraVoice.Native;

namespace TerraVoice.Core;

[Autoload(Side = ModSide.Client)]
internal sealed class VoiceInputSystem : ModSystem
{
    // The rnnoise library only works with a sampling rate of 48KHz.
    public const int SampleRate = 48_000;

    // Used for convenience as the opus default frame size is 20ms - https://wiki.xiph.org/Opus_Recommended_Settings.
    public const int MicrophoneInputDurationMs = 20;

    public bool HasValidAudioInput => audioDevices.Count > 0;

    public bool MicrophoneEnabled { get; set; }

    private VoiceProcessingSystem processingSystem;

    private ALMono16Microphone microphone;

    private List<string> audioDevices;

    private int deviceIndex;

    private bool recording;

    public override void Load()
    {
        audioDevices = ALMono16Microphone.GetDevices();

        SwitchAudioDevice(0);
    }

    public override void PostSetupContent()
    {
        processingSystem = ModContent.GetInstance<VoiceProcessingSystem>();
    }

    public override void Unload()
    {
        microphone?.Dispose();
        microphone = null;
    }

    public override void PreSaveAndQuit()
    {
        MicrophoneEnabled = false;
    }

    public override void PostUpdateEverything()
    {
        if (MicrophoneEnabled && !recording)
        {
            microphone.OnBufferReady += HandleAudioInputBuffer;
            microphone.StartRecording();

            recording = true;
        }

        if (!MicrophoneEnabled)
        {
            microphone.StopRecording();
            microphone.OnBufferReady -= HandleAudioInputBuffer;

            recording = false;
        }
    }

    private void HandleAudioInputBuffer(short[] buffer)
    {
        processingSystem.SubmitBuffer(buffer);
    }

    private void SwitchAudioDevice(int i)
    {
        // No audio input devices detected.
        if (audioDevices == null || audioDevices.Count == 0)
        {
            return;
        }

        if (microphone != null)
        {
            microphone.OnBufferReady -= HandleAudioInputBuffer;
            microphone.StopRecording();
            microphone.Dispose();
        }

        microphone = new ALMono16Microphone(audioDevices[i], MicrophoneInputDurationMs, SampleRate);

        if (recording)
        {
            microphone.OnBufferReady += HandleAudioInputBuffer;
            microphone.StartRecording();
        }
    }
}

