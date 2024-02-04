using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace TerraVoice.Core;

internal sealed class VoiceInputSystem : ModSystem
{
    // The rnnoise library only works with a sampling rate of 48KHz.
    public const int SampleRate = 48_000;

    // Needed due to the required P-Opus frame size being 20ms.
    public const int MicrophoneInputDurationMs = 20;

    private bool MicrophoneEnabled { get; set; }

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
        => processingSystem = ModContent.GetInstance<VoiceProcessingSystem>();

    public override void Unload()
    {
        microphone?.Dispose();
        microphone = null;
    }

    public override void PreSaveAndQuit()
        => MicrophoneEnabled = false;

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

    public override void PostUpdateInput()
    {
        if (Main.keyState.IsKeyDown(Keys.J) && Main.oldKeyState.IsKeyUp(Keys.J))
        {
            MicrophoneEnabled = !MicrophoneEnabled;
            Main.NewText("Mic: " + MicrophoneEnabled);
        }

        if (Main.keyState.IsKeyDown(Keys.L) && Main.oldKeyState.IsKeyUp(Keys.L))
        {
            deviceIndex++;

            if (deviceIndex >= audioDevices.Count)
            {
                deviceIndex = 0;
            }

            SwitchAudioDevice(deviceIndex);

            Main.NewText("Device: " + audioDevices[deviceIndex]);
        }
    }

    private void HandleAudioInputBuffer(short[] buffer)
        => processingSystem.SubmitBuffer(buffer);

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

