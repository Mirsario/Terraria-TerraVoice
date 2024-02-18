using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.UI;
using TerraVoice.UI.Abstract;

namespace TerraVoice.UI.ControlPanel;

internal class DeviceSwitchButton : SmartUIElement
{
    private const int ButtonWidth = 32;
    private const int ButtonHeight = 32;
    private const int PressDuration = 7;

    private readonly DeviceSwitcher switcher;

    private int pressedTimer;

    public DeviceSwitchButton(DeviceSwitcher switcher)
    {
        this.switcher = switcher;

        Width.Set(ButtonWidth, 0);
        Height.Set(ButtonHeight, 0);
    }

    public override void SafeMouseDown(UIMouseEvent evt)
    {
        switcher.NextAudioDevice();

        pressedTimer = PressDuration;
    }

    public override void SafeUpdate(GameTime gameTime)
    {
        pressedTimer--;

        if (pressedTimer < 0)
        {
            pressedTimer = 0;
        }
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Texture2D texture = ModAsset.DeviceSwitcherButton.Value;

        Vector2 position = GetDimensions().Position();

        Rectangle sourceRectangle = new(pressedTimer > 0 ? 0 : (texture.Width / 2), 0, texture.Width / 2, texture.Height);

        spriteBatch.Draw(texture, position - new Vector2(0, pressedTimer > 0 ? 0 : 4), sourceRectangle, Color.White);
    }
}
