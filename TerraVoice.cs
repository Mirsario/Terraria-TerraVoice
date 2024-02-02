using System.IO;
using Terraria;
using Terraria.ModLoader;
using TerraVoice.Misc;

namespace TerraVoice;

public partial class TerraVoice : Mod
{
    public static readonly string CachePath = Path.Combine(Main.SavePath, "TerraVoice");

    public static TerraVoice Instance { get; private set; }

    public override void Load() 
    {
        Instance = this;
    }

    public override void Unload() 
    {
        VoiceConfig.Instance = null;
        PersonalConfig.Instance = null;
    }
}