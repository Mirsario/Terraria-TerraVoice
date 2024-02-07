using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using TerraVoice.Core;
using TerraVoice.Systems;
using TerraVoice.UI.Abstract;

namespace TerraVoice.UI.ControlPanel;

internal class VoiceControlPanel : SmartUIElement
{
    private const int PanelWidth = 640;
    private const int PanelHeight = 448;

    private const int Spacing = 16;

    private const int MicrophoneSwitch = 0;
    private const int TestSwitch = 1;
    private const int NoiseSuppressionSwitch = 2;
    private const int IconSwitch = 3;

    private float oldScale;

    private readonly VoiceControlState parent;

    private readonly SwitchButton[] switches;

    public VoiceControlPanel(VoiceControlState parent)
    {
        this.parent = parent;

        switches = new SwitchButton[4];

        int y = Spacing;

        Texture2D[] icons = new Texture2D[]
        {
            ModAsset.Microphone.Value,
            ModAsset.Microphone.Value,
            ModAsset.Microphone.Value,
            ModAsset.Microphone.Value
        };

        // TODO: Localise this
        string[] labels = new string[]
        {
            "Mic",
            "Test",
            "Denoise",
            "No icons"
        };

        for (int i = 0; i < 4; i++)
        {
            SwitchButton panelSwitch = new(icons[i], labels[i]);
            panelSwitch.Left.Set(Spacing + ((Spacing + SwitchButton.SwitchWidth) * i), 0);
            panelSwitch.Top.Set(y, 0);
            Append(panelSwitch);

            switches[i] = panelSwitch;
        }

        y += (int)switches[0].Height.Pixels + Spacing;

        AudioVisualiserWidget audioVisualiser = new();
        audioVisualiser.Left.Set(Spacing, 0);
        audioVisualiser.Top.Set(y, 0);
        audioVisualiser.Width.Set(PanelWidth - (Spacing * 2), 0);
        audioVisualiser.Height.Set(56, 0);
        Append(audioVisualiser);
    }

    public override void SafeUpdate(GameTime gameTime)
    {
        VoiceInputSystem inputSystem = ModContent.GetInstance<VoiceInputSystem>();
        VoiceProcessingSystem processingSystem = ModContent.GetInstance<VoiceProcessingSystem>();
        IconDrawingSystem iconDrawingSystem = ModContent.GetInstance<IconDrawingSystem>();

        inputSystem.MicrophoneEnabled = switches[MicrophoneSwitch].Enabled;
        processingSystem.TestMode = switches[TestSwitch].Enabled;
        processingSystem.NoiseSuppression = switches[NoiseSuppressionSwitch].Enabled;
        iconDrawingSystem.NoIcons = switches[IconSwitch].Enabled;
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        if (GetDimensions().ToRectangle().Contains(Main.MouseScreen.ToPoint()))
        {
            Main.LocalPlayer.mouseInterface = true;
        }

        Vector2 position = GetDimensions().Position();

        DrawMainPanel(position, spriteBatch);

        CheckScaleChanges();

        base.DrawSelf(spriteBatch);
    }

    public override void Recalculate()
    {
        Width.Set(PanelWidth, 0);
        Height.Set(PanelHeight, 0);
        Left.Set(-Width.Pixels / 2, 0.5f);
        Top.Set(-Height.Pixels / 2, 0.5f);

        base.Recalculate();
    }

    private void CheckScaleChanges()
    {
        if (!Main.gamePaused && Main.UIScale != oldScale)
        {
            parent.Recalculate();

            oldScale = Main.UIScale;
        }
    }

    private void DrawMainPanel(Vector2 position, SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(ModAsset.MainPanel.Value, position, Color.White);
    }
}
