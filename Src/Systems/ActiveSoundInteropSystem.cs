using Microsoft.Xna.Framework.Audio;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria.Audio;
using Terraria.ModLoader;

namespace TerraVoice.Systems;

[Autoload(Side = ModSide.Client)]
internal class ActiveSoundInteropSystem : ModSystem
{
    public override void Load()
    {
        IL_ActiveSound.Play += LinkPlayerSpeakerToActiveSound;
    }
    
    private void LinkPlayerSpeakerToActiveSound(ILContext il)
    {
        ILCursor c = new(il);

        c.GotoNext(
            i => i.MatchCall(
                typeof(ActiveSound)
                .GetProperty("Style", BindingFlags.Public | BindingFlags.Instance)
                .GetGetMethod()
            )
        );

        c.RemoveRange(5);

        c.EmitDelegate<Func<ActiveSound, SoundEffectInstance>>(activeSound =>
        {
            string path = activeSound.Style.SoundPath;

            if (path.StartsWith(PlayerSpeaker.DummySound))
            {
                int whoAmI = (int)activeSound.Style.PitchVariance;

                DynamicSoundEffectInstance instance = ModContent.GetInstance<VoiceOutputSystem>().GetSoundEffectByPlayer(whoAmI);

                // If this is reached and is true, then the sound being played is a dummy sound and requires substitution.
                if (instance != null)
                {
                    return instance;
                }
            }

            // Default behaviour.
            return activeSound.Style.GetRandomSound().CreateInstance();
        });
    }

    /* 
     * Instructions targeted by this IL edit:
     * 
     * IL_0019: ldarg.0    
     * 
     * << ILCursor initial position. >>
     * 
     * IL_001A: call      instance valuetype Terraria.Audio.SoundStyle Terraria.Audio.ActiveSound::get_Style()
     * IL_001F: stloc.1
     * IL_0020: ldloca.s  V_1
     * IL_0022: call      instance class [FNA]Microsoft.Xna.Framework.Audio.SoundEffect Terraria.Audio.SoundStyle::GetRandomSound()
     * IL_0027: callvirt  instance class [FNA]Microsoft.Xna.Framework.Audio.SoundEffectInstance [FNA]Microsoft.Xna.Framework.Audio.SoundEffect::CreateInstance()
     * 
     * << All instructions from the initial position to here are deleted. >>
     * 
     * << Delegate insertion occurs here. >>
     * 
     * IL_002C: stloc.0
     * 
     * 
     * 
     * C# equivalent:
     * 
     * SoundEffectInstance soundEffectInstance = this.Style.GetRandomSound().CreateInstance();
     */
}
