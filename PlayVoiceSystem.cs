using NAudio.Wave;
using Steamworks;
using Terraria;
using Terraria.ModLoader;

namespace TerraVoice;

/// <summary>
/// 某个玩家的语音播放器
/// </summary>
/// <param name="WaveOut">播放器</param>
/// <param name="BufferedWaveProvider">5s缓存区</param>
public record PlayerSpeaker(WaveOutEvent WaveOut, BufferedWaveProvider BufferedWaveProvider)
{
    public PlayerSpeaker() : this(new WaveOutEvent(),
        new BufferedWaveProvider(new WaveFormat((int) PlayVoiceSystem.SampleRate, 1))) {
        WaveOut.Init(BufferedWaveProvider);
        WaveOut.Play();
    }
}

[Autoload(Side = ModSide.Client)]
public class PlayVoiceSystem : ModSystem
{
    private static readonly byte[] DataDecompressedBuffer = new byte[50 * 1024];
    private static PlayerSpeaker[] _playerSpeakers;
    internal const uint SampleRate = 22050; // 44100;

    /// <summary>
    /// 向音频缓存区中添加数据，不要将缓存区填满
    /// </summary>
    /// <param name="player">玩家编号</param>
    /// <param name="data">要填入的数据</param>
    /// <param name="position">数据的起始位置</param>
    /// <param name="len">数据的长度</param>
    public static void AddDataToBufferedWaveProvider(int player, byte[] data, int position, int len) {
        _playerSpeakers[player] ??= new PlayerSpeaker();
        _playerSpeakers[player].BufferedWaveProvider.AddSamples(data, position, len);
    }

    public override void Load() {
        _playerSpeakers = new PlayerSpeaker[Main.maxPlayers];
    }

    public override void Unload() {
        _playerSpeakers = null;
    }

    public static void ReceiveVoiceBuffer(int sender, byte[] buffer, uint dataSize) {
        SteamUser.DecompressVoice(buffer, dataSize, DataDecompressedBuffer,
            (uint) DataDecompressedBuffer.Length, out uint bytesWritten, SampleRate);
        AddDataToBufferedWaveProvider(sender, DataDecompressedBuffer, 0, (int) bytesWritten);
    }
}