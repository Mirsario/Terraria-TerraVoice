using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerraVoice.UI.Abstract;
using ReLogic.Graphics;
using Terraria.UI;
using Terraria;
using System;

namespace TerraVoice.UI.ControlPanel;

internal class Slider : SmartUIElement
{
    private readonly Ref<int> setting;

    private const int SliderWidth = 530;
    private const int SliderHeight = 32;
    private const int BaseHeight = 12;
    private const int KnobWidth = 12;

    private readonly int maxRange;

    private int sliderX;

    private bool sliding;

    public Slider(int maxRange, Ref<int> setting)
    {
        this.maxRange = maxRange;
        this.setting = setting;

        sliderX = (int)MathF.Ceiling(MathHelper.Lerp(0, SliderWidth - KnobWidth, setting.Value / (float)maxRange));

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

        setting.Value = (int)MathF.Floor((float)sliderX / (SliderWidth - KnobWidth) * maxRange);
    }

    private void DrawSlider(SpriteBatch spriteBatch, Rectangle drawBox)
    {
        int gap = (drawBox.Height - BaseHeight) / 2;

        Vector2 sliderBasePosition = new(drawBox.X, drawBox.Y + gap);

        spriteBatch.Draw(ModAsset.Slider.Value, sliderBasePosition, Color.White);

        spriteBatch.Draw(ModAsset.RangeMarks.Value, sliderBasePosition - new Vector2(0, 8), Color.White);

        Vector2 sliderKnobPosition = new(drawBox.X + sliderX, drawBox.Y - 6);

        spriteBatch.Draw(ModAsset.SliderKnob.Value, sliderKnobPosition, Color.White);
    }

    private void DrawIndicator(SpriteBatch spriteBatch, Rectangle drawBox)
    {
        int x = drawBox.X + drawBox.Width + VoiceControlPanel.Spacing - 2;

        Rectangle indicatorBox = new(x, drawBox.Y, 64, drawBox.Height);

        spriteBatch.Draw(ModAsset.RangeWidget.Value, new Vector2(indicatorBox.X, indicatorBox.Y), Color.White);

        string text = setting.Value == 0 ? "Inf." : setting.Value.ToString();

        Vector2 boxMiddle = new(indicatorBox.X + (indicatorBox.Width / 2), indicatorBox.Y + (indicatorBox.Height / 2));

        Vector2 halfTextSize = TerraVoice.Font.MeasureString(text).RoundEven() / 2;

        spriteBatch.DrawString(TerraVoice.Font, text, boxMiddle - halfTextSize, TerraVoice.Cyan);
    }
}
