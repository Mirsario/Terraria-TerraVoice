using System;
using System.Linq;
using Terraria.Audio;
using Terraria.ModLoader;

namespace TerraVoice.Misc;

public class BgmFadeSystem : ModSystem
{
    private static float _bgmFadeOut;

    public override void Load() {
        On_LegacyAudioSystem.UpdateCommonTrack += (On_LegacyAudioSystem.orig_UpdateCommonTrack orig,
            LegacyAudioSystem self, bool active, int i, float volume, ref float fade) => {
            volume *= 1f - _bgmFadeOut;
            orig.Invoke(self, active, i, volume, ref fade);
        };

        On_LegacyAudioSystem.UpdateCommonTrackTowardStopping +=
        (On_LegacyAudioSystem.orig_UpdateCommonTrackTowardStopping orig, LegacyAudioSystem self, int i,
            float volume, ref float fade, bool audible) => {
            volume *= 1f - _bgmFadeOut;
            orig.Invoke(self, i, volume, ref fade, audible);
        };
    }

    public override void PostUpdateTime() {
        if (VoiceConfig.Instance.VoiceAttenuation || !PersonalConfig.Instance.BgmFadeOut) {
            _bgmFadeOut = 0f;
            return;
        }

        bool anyPlayerSpeaking = DrawingSystem.PlayerSpeaking.Any(s => s > 0);
        if (anyPlayerSpeaking)
            _bgmFadeOut += 0.06f;
        else
            _bgmFadeOut -= 0.02f;

        _bgmFadeOut = Math.Clamp(_bgmFadeOut, 0f, 0.5f);
    }
}