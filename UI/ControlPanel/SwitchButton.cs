using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
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
        this.label = label;

        Width.Set(SwitchWidth, 0);
        Height.Set(SwitchHeight, 0);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        Vector2 position = GetDimensions().Position();

        Texture2D texture = Enabled ? ModAsset.Switch_On.Value : ModAsset.Switch_Off.Value;

        spriteBatch.Draw(texture, position, Color.White);

        // Offset needs to be an even number to prevent weird scaling issues.
        float stringWidth = TerraVoice.Font.MeasureString(label).RoundEven().X;
        Vector2 textPosition = position + new Vector2((Width.Pixels / 2) - (stringWidth / 2), Height.Pixels - 12);

        spriteBatch.DrawString(TerraVoice.Font, label, textPosition, TerraVoice.Pink);
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
}
