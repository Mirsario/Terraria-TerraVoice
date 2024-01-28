using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TerraVoice.Misc;

// 阉割版隐藏了声音衰减功能
public class VoiceConfig
{
    [DefaultValue(false)]
    public bool VoiceAttenuation;

    [Range(30, 150)]
    [DefaultValue(90)]
    [Slider]
    [Increment(5)]
    public int VoiceAttenuationDistance;
    
    public static VoiceConfig Instance = new ();
}