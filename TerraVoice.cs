using Terraria.ModLoader;

namespace TerraVoice;

public partial class TerraVoice : Mod
{
    public static TerraVoice Instance;

    public override void Load() {
        Instance = this;
    }
}