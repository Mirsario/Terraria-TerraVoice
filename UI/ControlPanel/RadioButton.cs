using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.UI;
using TerraVoice.UI.Abstract;

namespace TerraVoice.UI.ControlPanel;

internal class RadioButton : SmartUIElement
{
    public const int ButtonWidth = 56;
    public const int ButtonHeight = 56;

    public bool Enabled { get; private set; }

    private readonly List<RadioButton> buttons;

    public RadioButton(List<RadioButton> buttons, bool enabled)
    {
        this.buttons = buttons;

        buttons.Add(this);

        Enabled = enabled;

        Width.Set(ButtonWidth, 0);
        Height.Set(ButtonHeight, 0);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Rectangle drawBox = GetDimensions().ToRectangle();

        spriteBatch.Draw(TextureAssets.MagicPixel.Value, drawBox, Enabled ? TerraVoice.Cyan : TerraVoice.Pink);
    }

    public override void SafeMouseDown(UIMouseEvent evt)
    {
        if (Enabled)
        {
            return;
        }

        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i] != this)
            {
                buttons[i].Enabled = Enabled;
            }
        }

        Enabled = !Enabled;

        SoundStyle sound = new("TerraVoice/Assets/Sounds/UI/SwitchOn")
        {
            Volume = 0.5f,
            Pitch = 0.8f
        };

        SoundEngine.PlaySound(sound);
    }
}
