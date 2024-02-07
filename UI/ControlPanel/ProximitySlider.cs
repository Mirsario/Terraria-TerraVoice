using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using TerraVoice.UI.Abstract;
using ReLogic.Graphics;
using System;
using Terraria.UI;
using Terraria;

namespace TerraVoice.UI.ControlPanel;

internal class ProximitySlider : SmartUIElement
{
    private const int SliderWidth = 528;
    private const int SliderHeight = 32;
    private const int BaseHeight = 12;
    private const int KnobWidth = 12;
    private const int MaxRange = 96;

    private int sliderX;

    private bool sliding;

    public ProximitySlider()
    {
        Width.Set(SliderWidth, 0);
        Height.Set(SliderHeight, 0);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Rectangle drawBox = GetDimensions().ToRectangle();

        DrawSlider(spriteBatch, drawBox);
        DrawIndicator(spriteBatch, drawBox);
    }

    public override void SafeMouseDown(UIMouseEvent evt)
    {
        Rectangle drawBox = GetDimensions().ToRectangle();

        if (drawBox.Contains(Main.MouseScreen.ToPoint()))
        {
            sliding = true;
        }
    }

    public override void SafeUpdate(GameTime gameTime)
    {
        Rectangle drawBox = GetDimensions().ToRectangle();

        if (sliding)
        {
            sliderX = (int)Main.MouseScreen.X - drawBox.X;
        }

        if (!Main.mouseLeft)
        {
            sliding = false;
        }

        sliderX = (int)MathHelper.Clamp(sliderX, 0, drawBox.Width - KnobWidth);
    }

    private void DrawSlider(SpriteBatch spriteBatch, Rectangle drawBox)
    {
        int yGap = (drawBox.Height - BaseHeight) / 2;

        Rectangle baseArea = new(drawBox.X, drawBox.Y + yGap, SliderWidth, drawBox.Height - (yGap * 2));

        spriteBatch.Draw(TextureAssets.MagicPixel.Value, baseArea, TerraVoice.Cyan);

        Rectangle slider = new(drawBox.X + sliderX, drawBox.Y, KnobWidth, drawBox.Height);

        spriteBatch.Draw(TextureAssets.MagicPixel.Value, slider, TerraVoice.Pink);
    }

    private void DrawIndicator(SpriteBatch spriteBatch, Rectangle drawBox)
    {
        int x = drawBox.X + drawBox.Width + VoiceControlPanel.Spacing;

        Rectangle indicatorBox = new(x, drawBox.Y, 64, drawBox.Height);

        spriteBatch.Draw(TextureAssets.MagicPixel.Value, indicatorBox, TerraVoice.Pink);

        int range = (int)Math.Floor((float)sliderX / (SliderWidth - KnobWidth) * MaxRange);

        string text = range == 0 ? "Inf." : range.ToString();

        Vector2 boxMiddle = new(indicatorBox.X + (indicatorBox.Width / 2), indicatorBox.Y + (indicatorBox.Height / 2));

        Vector2 halfTextSize = TerraVoice.Font.MeasureString(text).RoundEven() / 2;

        spriteBatch.DrawString(TerraVoice.Font, text, boxMiddle - halfTextSize, TerraVoice.Cyan);
    }
}
