using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.UI;
using TerraVoice.UI.Abstract;

namespace TerraVoice.UI.ControlPanel;

internal class SwitchButton : SmartUIElement
{
    public bool Enabled { get; private set; }

    public const int SwitchWidth = 140;
    public const int SwitchHeight = 120;

    private readonly Texture2D icon;

    private readonly string label;

    public SwitchButton(Texture2D icon, string label)
    {
        this.icon = icon;
        this.label = Language.GetTextValue($"Mods.TerraVoice.UI.{label}");

        Width.Set(SwitchWidth, 0);
        Height.Set(SwitchHeight, 0);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Vector2 position = GetDimensions().Position();

        Texture2D texture = Enabled ? ModAsset.Switch_On.Value : ModAsset.Switch_Off.Value;

        spriteBatch.Draw(texture, position, Color.White);

        DrawLabel(spriteBatch, position);
        DrawIcon(spriteBatch, position);
    }

    public override void SafeMouseDown(UIMouseEvent evt)
    {
        Enabled = !Enabled;

        SoundStyle sound = new($"TerraVoice/Assets/Sounds/UI/Switch{(Enabled ? "On" : "Off")}")
        {
            Volume = 0.5f
        };

        SoundEngine.PlaySound(sound);
    }

    private void DrawLabel(SpriteBatch spriteBatch, Vector2 position)
    {
        // Offset needs to be an even number to prevent weird scaling issues.
        float stringWidth = TerraVoice.Font.MeasureString(label).X;
        Vector2 textPosition = position + new Vector2((Width.Pixels / 2) - (stringWidth / 2), Height.Pixels - 3);

        spriteBatch.DrawString(TerraVoice.Font, label, textPosition, TerraVoice.Pink);
    }

    private void DrawIcon(SpriteBatch spriteBatch, Vector2 position)
    {
        Vector2 iconMiddle = position + new Vector2(99, 80);
        Vector2 halfIconSize = new(icon.Width / 2, icon.Height / 2);

        spriteBatch.Draw(icon, iconMiddle - halfIconSize, Color.White);
    }
}
