﻿using System;
using System.Runtime.InteropServices;

namespace TerraVoice.Native;

using DenoiseState = IntPtr;
using RNNModel = IntPtr;
using FilePtr = IntPtr;

/// <summary>
/// Wrapper class for the rnnoise native library.
/// This library is responsible for noise suppression concerning 48KHz 16-bit PCM audio data.
/// </summary>
internal sealed class rnnoise
{
    private const string DllPath = "librnnoise.dll";

    private const int rnnoise_frame_size = 480;

    private static DenoiseState denoiseState;

    private static readonly float[] ProcessBuffer = new float[rnnoise_frame_size];

    public static void rnnoise_create()
    {
        denoiseState = rnnoise_create(RNNModel.Zero);
    }

    // A few things about rnnoise are important to understand this - it can only process float data (but with a range of -32767..32767).
    // Additionally, it only works in chunks of sample length 480 (10ms).
    // Since the audio format is 48KHz 16-bit PCM, it has to first interpret a byte array as a short[], before casting to a float and splitting it into chunks.
    // These chunks are then individually de-noised and converted back into shorts, with the unsafe code allowing seamless converstion back to a byte[].
    public static void rnnoise_process_frame(byte[] buffer)
    {
        int toRead = buffer.Length / 2;
        int offset = 0;

        unsafe
        {
            fixed (byte* bufferPointer = buffer)
            {
                short* pcmSamples = (short*)bufferPointer;

                while (toRead > 0)
                {
                    int nextRead = Math.Min(toRead, rnnoise_frame_size);

                    for (int i = 0; i < nextRead; i++)
                    {
                        ProcessBuffer[i] = pcmSamples[offset + i];
                    }

                    if (nextRead < rnnoise_frame_size)
                        Array.Clear(ProcessBuffer, nextRead, rnnoise_frame_size - nextRead);

                    rnnoise_process_frame(denoiseState, ProcessBuffer, ProcessBuffer);

                    for (int i = 0; i < nextRead; i++)
                    {
                        pcmSamples[offset + i] = (short)ProcessBuffer[i];
                    }

                    toRead -= nextRead;
                    offset += nextRead;
                }
            }
        }
    }

    public static void rnnoise_destroy()
    {
        rnnoise_destroy(denoiseState);
    }

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    private static extern int rnnoise_init(DenoiseState st, RNNModel model);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    private static extern DenoiseState rnnoise_create(RNNModel model);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    private static extern void rnnoise_destroy(DenoiseState st);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    private static extern float rnnoise_process_frame(DenoiseState st, float[] outStream, float[] inStream);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    private static extern RNNModel rnnoise_model_from_file(FilePtr f);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    private static extern void rnnoise_model_free(RNNModel model);
}
