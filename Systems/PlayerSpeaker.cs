using Microsoft.Xna.Framework.Audio;
using System;

namespace TerraVoice.Core;

public class PlayerSpeaker : IDisposable
{
    public float Volume
    {
        get => soundEffectInstance.Volume;
        set => soundEffectInstance.Volume = value;
    }

    public float Pan
    {
        get => soundEffectInstance.Pan;
        set => soundEffectInstance.Pan = value;
    }

    private DynamicSoundEffectInstance soundEffectInstance;

    public PlayerSpeaker() {

        soundEffectInstance = new DynamicSoundEffectInstance(VoiceInputSystem.SampleRate, AudioChannels.Mono);
        soundEffectInstance.Play();
    }

    public void SubmitBuffer(byte[] data) => soundEffectInstance.SubmitBuffer(data);

    public void Reset() 
    {
        soundEffectInstance.Stop();
        soundEffectInstance.Play();
    }

    public void Dispose() 
    {
        soundEffectInstance?.Dispose();
        soundEffectInstance = null;
    }
}