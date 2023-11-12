using System;
using System.Linq;
using Steamworks;
using Terraria;
using Terraria.ModLoader;

namespace TerraVoice;

[Autoload(Side = ModSide.Client)]
public class PushToTalkContoller : ModPlayer
{
    private static readonly byte[] VoiceDataBuffer = new byte[10000];
    private static ModKeybind _pushToTalk;

    public override void Load() {
        _pushToTalk = KeybindLoader.RegisterKeybind(Mod, "PushToTalk", "P");
    }

    public override void SetControls() {
        if (_pushToTalk.JustPressed) {
            SteamUser.StartVoiceRecording();
            Main.NewText("Voice Recording Started");
        }

        if (_pushToTalk.JustReleased) {
            SteamUser.StopVoiceRecording();
            Main.NewText("Voice Recording Stopped");
        }

        if (SteamUser.GetAvailableVoice(out var dataSize) is EVoiceResult.k_EVoiceResultOK && dataSize > 0) {
            SteamUser.GetVoice(true, VoiceDataBuffer, dataSize, out _);
            TerraVoice.PushVoiceBuffer(VoiceDataBuffer, dataSize);
            // if (data.Any(d => d is not 0)) {
            //     Main.NewText("Yes");
            // }
        }
    }
}