using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.UI;
using TerraVoice.UI.Abstract;

namespace TerraVoice.UI.ControlPanel;

internal class Knob : SmartUIElement
{
    public const int KnobWidth = 128;
    public const int KnobHeight = 128;

    public float Volume => (float)MathHelper.Lerp(min, max, Math.Abs(InverseLerp(MinAngle, MaxAngle, angle)));

    private const float MinAngle = MathHelper.Pi + MathHelper.PiOver2;
    private const float MaxAngle = -MathHelper.PiOver4;

    private const float SpinSensitivity = 2;

    private float CursorOffsetFromCenterX => Main.MouseScreen.X - (GetDimensions().Position().X + GetDimensions().Width / 2);

    private float angle;

    private bool dragging;

    private float startOffset;

    private float startAngle;

    private int spriteX;
    private int oldSpriteX;

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

        Vector2 position = GetDimensions().Position();

        DrawMarkings(spriteBatch, position);

        spriteBatch.Draw(ModAsset.Knob.Value, position, Color.White);

        DrawIndicator(spriteBatch, position);
    }

    public override void SafeUpdate(GameTime gameTime)
    {
        PlayerInput.LockVanillaMouseScroll("TerraVoice/VoiceControlState");

        if (!Main.mouseLeft && dragging)
        {
            dragging = false;
            startOffset = 0;
        }

        if (dragging)
        {
            float currentOffset = CursorOffsetFromCenterX - startOffset;

            float radiansOffset = -MathHelper.ToRadians(currentOffset) * SpinSensitivity;

            angle = startAngle + radiansOffset;
        }

        angle = MathHelper.Clamp(angle, MaxAngle, MinAngle);

        if (spriteX != oldSpriteX)
        {
            PlaySound();

            oldSpriteX = spriteX;
        }

        base.SafeUpdate(gameTime);
    }

    public override void SafeScrollWheel(UIScrollWheelEvent evt)
    {
        /*int sign = Math.Sign(evt.ScrollWheelValue);

        float oldAngle = angle;

        angle += (MaxAngle - MinAngle) / 7 * sign;

        angle = MathHelper.Clamp(angle, MaxAngle, MinAngle);

        if (oldAngle != angle)
        {
            PlaySound();
        }*/
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

    private void DrawIndicator(SpriteBatch spriteBatch, Vector2 position)
    {
        Texture2D turns = ModAsset.KnobTurns.Value;

        spriteX = (int)Math.Ceiling(InverseLerp(MinAngle, MaxAngle, angle) * 7);

        Rectangle sourceRectangle = new(turns.Width / 8 * spriteX, 0, turns.Width / 8, turns.Height);

        spriteBatch.Draw(turns, position, sourceRectangle, Color.White);
    }

    private void DrawMarkings(SpriteBatch spriteBatch, Vector2 position)
    {
        Texture2D markings = ModAsset.KnobMarkings.Value;

        Vector2 halfSize = markings.Size() / 2;

        spriteBatch.Draw(ModAsset.KnobMarkings.Value, position + halfSize, null, Color.White, angle, halfSize, 1, SpriteEffects.None, 0);
    }

    private void PlaySound()
    {
        SoundStyle sound = new("TerraVoice/Assets/Sounds/UI/SwitchOn")
        {
            Volume = 0.5f,
            Pitch = 0.8f
        };

        SoundEngine.PlaySound(sound);
    }
}
