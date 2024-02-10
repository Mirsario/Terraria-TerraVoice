using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using TerraVoice.Core;
using TerraVoice.UI.Abstract;

namespace TerraVoice.UI.ControlPanel;

internal class AudioVisualiserWidget : SmartUIElement
{
    private readonly short[] testBuffer;

    public AudioVisualiserWidget()
    {
        testBuffer = new short[(int)(VoiceInputSystem.SampleRate * (VoiceInputSystem.MicrophoneInputDurationMs / 1000f))];

        ModContent.GetInstance<VoiceProcessingSystem>().OnTestBufferReceived += SubmitTestBuffer;
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Vector2 position = GetDimensions().Position();

        spriteBatch.Draw(ModAsset.AudioVisualiserWidget.Value, position, Color.White);

        DrawScreen(spriteBatch, position);
    }

    private void DrawAudioBars(SpriteBatch spriteBatch, Vector2 position)
    {
        int blocks = ((int)Width.Pixels - 16) / 4;
        int currentBlock = 0;
        int visualiserHeight = (int)Height.Pixels - 8;
        int sensitivity = 6;
        int minHeight = 4;

        for (int offset = 0; offset < Width.Pixels - 16; offset += 6)
        {
            int positionStart = (int)((float)currentBlock / blocks * testBuffer.Length);
            int blockLength = testBuffer.Length / blocks;

            float value = (float)Average(testBuffer, positionStart, blockLength) / short.MaxValue;

            int height = (int)(value * (visualiserHeight - minHeight) * sensitivity) + minHeight;
            height = (int)MathHelper.Clamp(height, 4, visualiserHeight);

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)position.X + offset, (int)position.Y + visualiserHeight / 2 - height / 2, 4, height / 2), TerraVoice.Pink);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)position.X + offset, (int)position.Y + visualiserHeight / 2, 4, height / 2), TerraVoice.Pink);

            currentBlock++;
        }
    }

    private short Average(short[] array, int start, int length)
    {
        int total = 0;

        for (int i = start; i < start + length; i++)
        {
            total += array[i];
        }

        return (short)(total / length);
    }

    // For visualisation >20ms maybe use a longer queue structure instead of a single array.
    private void SubmitTestBuffer(short[] buffer)
    {
        Buffer.BlockCopy(buffer, 0, testBuffer, 0, buffer.Length * sizeof(short));
    }

    private void DrawScreen(SpriteBatch spriteBatch, Vector2 position)
    {
        Vector2 middle = position + new Vector2(Width.Pixels / 2, Height.Pixels / 2);

        VoiceInputSystem inputSystem = ModContent.GetInstance<VoiceInputSystem>();
        VoiceProcessingSystem processingSystem = ModContent.GetInstance<VoiceProcessingSystem>();

        string text = "";

        if (!inputSystem.HasValidAudioInput)
        {
            text = Language.GetTextValue("Mods.TerraVoice.UI.NoInputDevice");
        }
        else if (!processingSystem.TestMode || !inputSystem.MicrophoneEnabled)
        {
            text = Language.GetTextValue("Mods.TerraVoice.UI.EnableTestMode");
        }

        // Offset needs to be an even number to prevent weird scaling issues.
        Vector2 halfTextSize = TerraVoice.Font.MeasureString(text).RoundEven() / 2;

        spriteBatch.DrawString(TerraVoice.Font, text, middle - halfTextSize, TerraVoice.Cyan);

        if (processingSystem.TestMode && inputSystem.HasValidAudioInput && inputSystem.MicrophoneEnabled)
        {
            DrawAudioBars(spriteBatch, position + new Vector2(4));
        }

        spriteBatch.DrawString(TerraVoice.Font, text, middle - halfTextSize, TerraVoice.Cyan);
    }

    private void DrawLabel(SpriteBatch spriteBatch, Vector2 position)
    {
        string label = Language.GetTextValue("Mods.TerraVoice.UI.AudioVisualizer");

        float labelWidth = TerraVoice.Font.MeasureString(label).X;

        Vector2 labelPosition = position + new Vector2((Width.Pixels / 2) - (labelWidth / 2), Height.Pixels - 3);

        spriteBatch.DrawString(TerraVoice.Font, label, labelPosition, TerraVoice.Pink);
    }
}
