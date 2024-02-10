using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using TerraVoice.UI.Abstract;

namespace TerraVoice.UI.ControlPanel;

internal class Knob : SmartUIElement
{
    public const int KnobWidth = 56;
    public const int KnobHeight = 56;

    public float Volume => (float)MathHelper.Lerp(min, max, Math.Abs(InverseLerp(MinAngle, MaxAngle, angle)));

    private const float MinAngle = MathHelper.Pi + MathHelper.PiOver4;
    private const float MaxAngle = -MathHelper.PiOver4;

    private float CursorOffsetFromCenterX => Main.MouseScreen.X - (GetDimensions().Position().X + GetDimensions().Width / 2);

    private float angle;

    private bool dragging;

    private float startOffset;

    private float startAngle;

    private readonly float min;
    private readonly float max;

    public Knob(float min, float max)
    {
        this.min = min;
        this.max = max;

        angle = MinAngle;

        Width.Set(KnobWidth, 0);
        Height.Set(KnobHeight, 0);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Rectangle drawBox = GetDimensions().ToRectangle();

        spriteBatch.Draw(TextureAssets.MagicPixel.Value, drawBox, TerraVoice.Pink);

        DrawIndicator(spriteBatch, drawBox);
    }

    public override void SafeUpdate(GameTime gameTime)
    {
        if (!Main.mouseLeft && dragging)
        {
            dragging = false;
            startOffset = 0;
        }

        if (dragging)
        {
            float currentOffset = CursorOffsetFromCenterX - startOffset;

            float radiansOffset = -MathHelper.ToRadians(currentOffset / 1.5f);

            angle = startAngle + radiansOffset;
        }

        angle = MathHelper.Clamp(angle, MaxAngle, MinAngle);

        base.SafeUpdate(gameTime);
    }

    public override void SafeMouseDown(UIMouseEvent evt)
    {
        Rectangle drawBox = GetDimensions().ToRectangle();

        if (drawBox.Contains((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y))
        {
            dragging = true;

            startOffset = CursorOffsetFromCenterX;
            startAngle = angle;
        }
    }

    private float InverseLerp(float a, float b, float value) => (value - a) / (b - a);

    private void DrawIndicator(SpriteBatch spriteBatch, Rectangle drawBox)
    {
        Vector2 origin = ModAsset.Mic.Value.Size() / 2;

        spriteBatch.Draw(ModAsset.Mic.Value, drawBox.TopLeft() + (drawBox.Size() / 2), null, Color.White, MathHelper.PiOver2 - angle, origin, 1, SpriteEffects.None, 0);
    }
}
