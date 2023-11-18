using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TerraVoice.Misc;

public class VoiceConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    [DefaultValue(false)]
    public bool VoiceAttenuation;

    [Range(30, 150)]
    [DefaultValue(90)]
    [Slider]
    [Increment(5)]
    public int VoiceAttenuationDistance;
    
    public static VoiceConfig Instance;

    public VoiceConfig() {
        Instance = this;
    }
}