using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using TerraVoice.UI.Abstract;

namespace TerraVoice.UI.ControlPanel;

internal class VoiceControlPanel : SmartUIElement
{
    public const int MicrophoneSwitch = 0;
    public const int TestSwitch = 1;
    public const int NoiseSuppressionSwitch = 2;
    public const int IconSwitch = 3;

    public const int OpenMicRadioButton = 0;
    public const int PushToTalkRadioButton = 1;

    public const int Spacing = 16;

    private const int PanelWidth = 640;
    private const int PanelHeight = 448;

    public SwitchButton[] Switches { get; private set; }

    public Knob Amplification { get; private set; }

    public Knob Threshold { get; private set; }

    public List<RadioButton> RadioButtons { get; private set; }

    private float oldScale;

    private readonly VoiceControlState parent;

    public VoiceControlPanel(VoiceControlState parent)
    {
        this.parent = parent;

        Recalculate();

        Switches = new SwitchButton[4];
        RadioButtons = new();

        int y = Spacing;

        InitialiseSwitches(y);

        y += (int)Switches[0].Height.Pixels + Spacing + 4;

        AudioVisualiserWidget audioVisualiser = new();
        audioVisualiser.Left.Set(Spacing, 0);
        audioVisualiser.Top.Set(y + 4, 0);
        audioVisualiser.Width.Set(PanelWidth - (Spacing * 2), 0);
        audioVisualiser.Height.Set(56, 0);
        Append(audioVisualiser);

        y += (int)audioVisualiser.Height.Pixels + Spacing;

        // TODO: Replace magic number with max proximity range.
        Slider slider = new(96);
        slider.Left.Set(Spacing, 0);
        slider.Top.Set(y, 0);
        Append(slider);

        y += (int)slider.Height.Pixels + Spacing;

        Amplification = new(0.5f, 2.25f);
        Amplification.Left.Set(Spacing, 0);
        Amplification.Top.Set(y, 0);
        Append(Amplification);

        Threshold = new(-10, 60);
        Threshold.Left.Set((Spacing * 2) + Knob.KnobWidth, 0);
        Threshold.Top.Set(y, 0);
        Append(Threshold);

        RadioButton openMic = new(RadioButtons, true, ModAsset.OpenMic.Value);
        openMic.Left.Set(Width.Pixels - Spacing - RadioButton.ButtonWidth, 0);
        openMic.Top.Set(y, 0);
        Append(openMic);

        y += (int)openMic.Height.Pixels + Spacing;

        RadioButton pushToTalk = new(RadioButtons, false, ModAsset.PushToTalk.Value);
        pushToTalk.Left.Set(Width.Pixels - Spacing - RadioButton.ButtonWidth, 0);
        pushToTalk.Top.Set(y, 0);
        Append(pushToTalk);
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

        DrawRadioButtonIndicators(spriteBatch);
    }

    private void InitialiseSwitches(int y)
    {
        int index = 0;

        AddSwitch(index++, y, ModAsset.Mic.Value, "Mic");
        AddSwitch(index++, y, ModAsset.Test.Value, "Test");
        AddSwitch(index++, y, ModAsset.Denoise.Value, "Denoise");
        AddSwitch(index, y, ModAsset.NoIcons.Value, "NoIcons");
    }

    private void AddSwitch(int i, int y, Texture2D icon, string label)
    {
        SwitchButton panelSwitch = new(icon, label);
        panelSwitch.Left.Set(Spacing + ((Spacing + SwitchButton.SwitchWidth) * i), 0);
        panelSwitch.Top.Set(y, 0);
        Switches[i] = panelSwitch;
        Append(panelSwitch);
    }

    private void DrawRadioButtonIndicators(SpriteBatch spriteBatch)
    {
        Texture2D indicator = ModAsset.Indicator.Value;

        foreach (RadioButton button in RadioButtons)
        {
            Vector2 buttonPosition = button.GetDimensions().Position();

            buttonPosition += new Vector2(-40, 10);

            Rectangle sourceRectangle = new(button.Enabled ? (indicator.Width / 2) : 0, 0, indicator.Width / 2, indicator.Height);

            spriteBatch.Draw(ModAsset.Indicator.Value, buttonPosition, sourceRectangle, Color.White);
        }
    }
}
