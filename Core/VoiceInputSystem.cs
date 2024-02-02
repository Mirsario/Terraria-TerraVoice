using Microsoft.Xna.Framework.Input;
using NAudio.Wave;
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

    private WaveInEvent waveIn;

    private bool recording;

    // TODO: Allow the audio device to be selected?
    // Enumerating them is annoying as most solutions only work on windows.
    public override void Load()
        => SwitchAudioDevice(0);

    public override void PostSetupContent()
        => processingSystem = ModContent.GetInstance<VoiceProcessingSystem>();

    public override void Unload()
        => waveIn?.Dispose();

    public override void PreSaveAndQuit()
        => MicrophoneEnabled = false;

    public override void PostUpdateEverything()
    {
        if (MicrophoneEnabled && !recording)
        {
            waveIn.DataAvailable += HandleAudioInputBuffer;
            waveIn.StartRecording();

            recording = true;
        }

        if (!MicrophoneEnabled)
        {
            waveIn.StopRecording();
            waveIn.DataAvailable -= HandleAudioInputBuffer;

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
    }

    private void HandleAudioInputBuffer(object sender, WaveInEventArgs args)
        => processingSystem.SubmitBuffer(args.Buffer);

    private void SwitchAudioDevice(int i)
    {
        if (waveIn != null)
        {
            waveIn.DataAvailable -= HandleAudioInputBuffer;
            waveIn.StopRecording();
            waveIn.Dispose();
        }

        waveIn = new WaveInEvent
        {
            DeviceNumber = i,
            WaveFormat = new WaveFormat(SampleRate, 16, 1),
            BufferMilliseconds = MicrophoneInputDurationMs
        };

        if (recording)
        {
            waveIn.DataAvailable += HandleAudioInputBuffer;
            waveIn.StartRecording();
        }
    }
}

