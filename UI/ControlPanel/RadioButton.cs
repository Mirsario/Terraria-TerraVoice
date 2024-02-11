using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.UI;
using TerraVoice.UI.Abstract;

namespace TerraVoice.UI.ControlPanel;

internal class RadioButton : SmartUIElement
{
    public const int ButtonWidth = 56;
    public const int ButtonHeight = 56;

    public bool Enabled { get; private set; }

    private readonly List<RadioButton> buttons;

    private readonly Texture2D icon;

    public RadioButton(List<RadioButton> buttons, bool enabled, Texture2D icon)
    {
        this.buttons = buttons;
        this.icon = icon;

        buttons.Add(this);

        Enabled = enabled;

        Width.Set(ButtonWidth, 0);
        Height.Set(ButtonHeight, 0);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Vector2 position = GetDimensions().Position();

        if (!Enabled)
        {
            position -= new Vector2(0, 8);
        }

        Rectangle sourceRectangle = new(Enabled ? 0 : (icon.Width / 2), 0, icon.Width / 2, icon.Height);

        spriteBatch.Draw(icon, position, sourceRectangle, Color.White);
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
