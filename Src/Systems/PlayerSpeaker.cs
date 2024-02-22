using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using Terraria.Audio;

namespace TerraVoice.Systems;

public class PlayerSpeaker : IDisposable
{
    public const string DummySound = "TerraVoice:PlayerSpeakerDummy";

    public DynamicSoundEffectInstance SoundEffectInstance { get; private set; }

    public float Volume
    {
        get => SoundEffectInstance.Volume;
        set => SoundEffectInstance.Volume = value;
    }

    public float Pan
    {
        get => SoundEffectInstance.Pan;
        set => SoundEffectInstance.Pan = value;
    }

    private readonly int whoAmI;

    private ActiveSound activeSound;

    public PlayerSpeaker(int whoAmI) 
    {
        this.whoAmI = whoAmI;

        SoundEffectInstance = new DynamicSoundEffectInstance(VoiceInputSystem.SampleRate, AudioChannels.Mono);
    }

    public void Dispose() 
    {
        SoundEffectInstance?.Dispose();
        SoundEffectInstance = null;
    }

    public void PlayAsActiveSound()
    {
        SoundStyle dummyStyle = new(DummySound)
        {
            PitchVariance = whoAmI
        };

        // The sound path passed in is that of a dummy sound.
        // This notifies the IL edit that the given sound should not be played.
        // Instead, the style's pitch variance (whoAmI) will be used to substitute in a PlayerSpeaker sound.
        activeSound = new ActiveSound(dummyStyle);
    }

    public void UpdatePosition(Vector2 playerPosition)
    {
        activeSound!.Position = playerPosition;
    }
}