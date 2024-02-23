using System;
using Terraria.Audio;
using Terraria.ModLoader;

namespace TerraVoice.Systems;

public class BgmFadeSystem : ModSystem
{
    private float bgmFadeOut;

    public override void Load()
    {
        On_LegacyAudioSystem.UpdateCommonTrack += ApplyDucking;
        On_LegacyAudioSystem.UpdateCommonTrackTowardStopping += ApplyDuckingTowardStopping;
    }

    public override void PostUpdateTime()
    {
        bool anyPlayerSpeaking = ModContent.GetInstance<IconDrawingSystem>().IsAnyPlayerSpeaking();

        if (anyPlayerSpeaking)
            bgmFadeOut += 0.06f;
        else
            bgmFadeOut -= 0.02f;

        bgmFadeOut = Math.Clamp(bgmFadeOut, 0f, 0.5f);
    }

    private void ApplyDucking(On_LegacyAudioSystem.orig_UpdateCommonTrack orig,
        LegacyAudioSystem self, bool active, int i, float volume, ref float fade)
    {
        volume *= 1f - bgmFadeOut;

        orig(self, active, i, volume, ref fade);
    }

    private void ApplyDuckingTowardStopping(
        On_LegacyAudioSystem.orig_UpdateCommonTrackTowardStopping orig,
        LegacyAudioSystem self, int i, float volume, ref float fade, bool audible)
    {
        volume *= 1f - bgmFadeOut;

        orig(self, i, volume, ref fade, audible);
    }
}