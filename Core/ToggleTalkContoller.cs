using System;
using Steamworks;
using Terraria.ModLoader;
using TerraVoice.Misc;

namespace TerraVoice.Core;

[Autoload(Side = ModSide.Client)]
public class ToggleTalkContoller : ModPlayer
{
    [Autoload(Side = ModSide.Client)]
    private class ModSystemForPreQuitHook : ModSystem
    {
        public override void PreSaveAndQuit() {
            if (SteamUser.GetAvailableVoice(out _) is not EVoiceResult.k_EVoiceResultNotRecording
                and not EVoiceResult.k_EVoiceResultNotInitialized)
                SteamUser.StopVoiceRecording();
        }
    }

    private static ModKeybind _talkKeybind;

    public override void Load() {
        _talkKeybind = KeybindLoader.RegisterKeybind(Mod, "TalkKeybind", "P");
    }

    public override void SetControls() {
        switch (PersonalConfig.Instance.TalkMode) {
            case TalkMode.Push:
                HandlePushToTalk();
                break;
            case TalkMode.Toggle:
                HandleToggle();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void HandleToggle() {
        if (!_talkKeybind.JustPressed) return;

        if (SteamUser.GetAvailableVoice(out _) is EVoiceResult.k_EVoiceResultNotRecording) {
            SteamUser.StartVoiceRecording();
        }
        else {
            SteamUser.StopVoiceRecording();
        }
    }

    private void HandlePushToTalk() {
        if (_talkKeybind.JustPressed) {
            SteamUser.StartVoiceRecording();
            // Main.NewText("Voice Recording Started");
        }

        if (_talkKeybind.JustReleased) {
            SteamUser.StopVoiceRecording();
            // Main.NewText("Voice Recording Stopped");
        }
    }
}