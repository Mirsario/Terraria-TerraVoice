using Microsoft.Xna.Framework;
using ReLogic.Content;
using ReLogic.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TerraVoice.Misc;
using TerraVoice.UI;
using TerraVoice.UI.ControlPanel;

namespace TerraVoice;

public partial class TerraVoice : Mod
{
    public static DynamicSpriteFont Font => Language.ActiveCulture.Name == "en-US" ? customFont : FontAssets.MouseText.Value;

    private static DynamicSpriteFont customFont;

    public static readonly string CachePath = Path.Combine(Main.SavePath, "TerraVoice");

    public static readonly Color Cyan = new(130, 233, 229);

    public static readonly Color Pink = new(226, 114, 175);

    private static ModKeybind voiceBind;

    public static TerraVoice Instance { get; private set; }

    public override void Load() 
    {
        Instance = this;

        if (!Main.dedServ)
        {
            voiceBind = KeybindLoader.RegisterKeybind(this, "VoiceControlPanel", "J");

            customFont = Assets.Request<DynamicSpriteFont>("Assets/Fonts/MP3-12", AssetRequestMode.ImmediateLoad).Value;
        }
    }

    public override void Unload() 
    {
        VoiceConfig.Instance = null;
        PersonalConfig.Instance = null;
    }

    [Autoload(Side = ModSide.Client)]
    private class KeybindPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (voiceBind.JustPressed)
            {
                VoiceControlState state = TerraVoiceUILoader.GetUIState<VoiceControlState>();

                state.Visible = !state.Visible;

                state.Recalculate();

                SoundEngine.PlaySound(state.Visible ? SoundID.MenuOpen : SoundID.MenuClose);
            }
        }
    }
}