using NAudio.Wave;
using TerraVoice.Misc;

namespace TerraVoice.Core;

public class WaveGraphRenderer
{
    public class VolumeSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider source;

        public VolumeSampleProvider(ISampleProvider source) {
            this.source = source;
            Volume = 1.0f;
        }

        public WaveFormat WaveFormat => source.WaveFormat;

        public int Read(float[] buffer, int offset, int sampleCount) {
            int samplesRead = source.Read(buffer, offset, sampleCount);
            bool showWave = PersonalConfig.Instance.ShowWave;
            for (int n = 0; n < sampleCount; n++) {
                if (showWave && n % 80 == 0) {
                    while (DrawingSystem.WaveDatas.Count > 600) {
                        DrawingSystem.WaveDatas.Dequeue();
                    }

                    DrawingSystem.WaveDatas.Enqueue(buffer[offset + n]);
                }
                buffer[offset + n] *= Volume;
            }

            return samplesRead;
        }

        public float Volume { get; set; }
    }

    private WaveOutEvent _waveOut;

    private readonly BufferedWaveProvider _bufferedWaveProvider;

    private readonly VolumeSampleProvider _waveProvider;

    public WaveGraphRenderer() {
        _waveOut = new WaveOutEvent();
        _bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat((int) PlayVoiceSystem.SampleRate, 1)) {
            DiscardOnBufferOverflow = true
        };
        _waveProvider = new VolumeSampleProvider(_bufferedWaveProvider.ToSampleProvider());
        _waveOut.Init(_waveProvider);
        _waveOut.Play();
    }

    public void AddSamples(byte[] data, int position, int len) {
        _bufferedWaveProvider.AddSamples(data, position, len);
        _waveProvider.Volume = PersonalConfig.Instance.HearYourself ? PersonalConfig.Instance.VoiceVolume : 0f;
    }

    public void Dispose() {
        _waveOut.Dispose();
        _waveOut = null;
    }
}