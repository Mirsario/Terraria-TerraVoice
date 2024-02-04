using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TerraVoice;

// Wrapper class for OpenTK's OpenAL bindings.
// This allows the audio input devices to be enumerated, and initialised.
// As well as this it will capture audio samples on a separate thread.
internal class ALMono16Microphone : IDisposable
{
    public event BufferReady OnBufferReady;

    private readonly int desiredFrameSize;

    private readonly ALCaptureDevice device;

    private readonly short[] buffer;

    private CancellationTokenSource source;

    public ALMono16Microphone(string microphone, int captureFrameDurationMs, int sampleRate)
    {
        desiredFrameSize = (int)(captureFrameDurationMs / 1000f * sampleRate);

        device = ALC.CaptureOpenDevice(microphone, sampleRate, ALFormat.Mono16, desiredFrameSize);

        buffer = new short[desiredFrameSize];
    }

    public void StartRecording()
    {
        ALC.CaptureStart(device);

        source = new();
        CancellationToken token = source.Token;

        Task.Factory.StartNew(() =>
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    ALC.CaptureStop(device);

                    Array.Clear(buffer);

                    return;
                }

                int samplesAvailable = ALC.GetInteger(device, AlcGetInteger.CaptureSamples);

                if (samplesAvailable >= desiredFrameSize)
                {
                    ALC.CaptureSamples(device, buffer, desiredFrameSize);

                    short[] copy = new short[desiredFrameSize];

                    Buffer.BlockCopy(buffer, 0, copy, 0, copy.Length * sizeof(short));

                    OnBufferReady?.Invoke(copy);
                }
            }
        }, source.Token);
    }

    public void StopRecording()
    {
        source?.Cancel();
    }

    public void Dispose()
    {
        source?.Cancel();

        ALC.CaptureCloseDevice(device);
    }

    public static List<string> GetDevices() 
        => ALC.GetString(ALDevice.Null, AlcGetStringList.CaptureDeviceSpecifier);

    public delegate void BufferReady(short[] buffer);
}
