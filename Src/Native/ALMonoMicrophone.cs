using Silk.NET.Core.Contexts;
using Silk.NET.Core.Loader;
using Silk.NET.OpenAL;
using Silk.NET.OpenAL.Extensions.EXT.Enumeration;
using Silk.NET.OpenAL.Extensions.EXT;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace TerraVoice.Native;

// Wrapper class for OpenTK's OpenAL bindings.
// This allows the audio input devices to be enumerated, and initialised.
// As well as this it will capture audio samples on a separate thread.
internal class ALMonoMicrophone : IDisposable
{
    public event BufferReady OnBufferReady;

    private static AL al;
    private static ALContext alc;
    private static Capture capture;

    private readonly int desiredFrameSize;

    private unsafe readonly Device* device;

    private readonly short[] buffer;

    private CancellationTokenSource source;

    public ALMonoMicrophone(string microphone, int captureFrameDurationMs, uint sampleRate)
    {
        desiredFrameSize = (int)(captureFrameDurationMs / 1000f * sampleRate);

        unsafe
        {
            device = capture.CaptureOpenDevice(microphone, sampleRate, BufferFormat.Mono16, desiredFrameSize);
        }

        buffer = new short[desiredFrameSize];
    }

    public void StartRecording()
    {
        unsafe
        {
            capture.CaptureStart(device);

            source = new();
            CancellationToken token = source.Token;

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        capture.CaptureStop(device);

                        Array.Clear(buffer);

                        return;
                    }

                    int samplesAvailable = capture.GetAvailableSamples(device);

                    if (samplesAvailable >= desiredFrameSize)
                    {
                        fixed (short* pBuffer = buffer)
                        {
                            capture.CaptureSamples(device, pBuffer, desiredFrameSize);
                        }

                        short[] copy = new short[desiredFrameSize];

                        Buffer.BlockCopy(buffer, 0, copy, 0, copy.Length * sizeof(short));

                        OnBufferReady?.Invoke(copy);
                    }
                }
            }, source.Token);
        }
    }

    public void StopRecording()
    {
        source?.Cancel();
    }

    public void Dispose()
    {
        source?.Cancel();

        unsafe
        {
            capture.CaptureCloseDevice(device);
        }
    }

    public static List<string> GetDevices()
    {
        unsafe
        {
            if (!alc.TryGetExtension(null, out CaptureEnumerationEnumeration enumeration))
            {
                throw new Exception("OpenAL EXT could not be found.");
            }

            return enumeration.GetStringList(GetCaptureContextStringList.CaptureDeviceSpecifiers).ToList();
        }
    }

    public static void LoadOpenAL(string openalPath)
    {
        UnmanagedLibrary lib = new(
            openalPath,
            LibraryLoader.GetPlatformDefaultLoader(),
            PathResolver.Default
        );

        DefaultNativeContext nativeContext = new(lib);

        al = new(nativeContext);
        alc = new(nativeContext);

        unsafe
        {
            if (!alc.TryGetExtension(null, out capture))
            {
                throw new Exception("OpenAL EXT could not be found.");
            }
        }
    }

    public static void UnloadOpenAL()
    {
        al.Dispose();
        alc.Dispose();
    }

    public delegate void BufferReady(short[] buffer);
}
