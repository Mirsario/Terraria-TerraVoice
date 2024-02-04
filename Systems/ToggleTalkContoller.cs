using System;
using Terraria.ModLoader;
using TerraVoice.Misc;

namespace TerraVoice.Core;

[Autoload(Side = ModSide.Client)]
public class ToggleTalkContoller : ModPlayer
{
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
    }

    private void HandlePushToTalk() {
        if (_talkKeybind.JustPressed) {
        }

        if (_talkKeybind.JustReleased) {
        }
    }
}