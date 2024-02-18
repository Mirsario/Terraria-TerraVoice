﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using TerraVoice.IO;
using TerraVoice.UI.Abstract;

namespace TerraVoice.UI.ControlPanel;

internal class VoiceControlPanel : SmartUIElement
{
    public const int Spacing = 16;

    private const int PanelWidth = 640;
    private const int PanelHeight = 448;

    public DualKnob ChannelAmplificationDualKnob { get; private set; }

    private readonly VoiceControlState parent;

    private readonly List<RadioButton> radioButtons;

    private float oldScale;

    public VoiceControlPanel(VoiceControlState parent)
    {
        UserDataStore data = PersistentDataStoreSystem.GetDataStore<UserDataStore>();

        this.parent = parent;

        Recalculate();

        radioButtons = new();

        int y = Spacing;

        int height = InitialiseSwitches(data, y);

        y += height + Spacing + 4;

        AudioVisualiserWidget audioVisualiser = new(this);
        audioVisualiser.Left.Set(Spacing, 0);
        audioVisualiser.Top.Set(y + 4, 0);
        audioVisualiser.Width.Set(PanelWidth - (Spacing * 2), 0);
        audioVisualiser.Height.Set(56, 0);
        Append(audioVisualiser);

        y += (int)audioVisualiser.Height.Pixels + Spacing;

        // TODO: Replace magic number with max proximity range.
        Slider rangeSlider = new(96, data.ProximityDistance);
        rangeSlider.Left.Set(Spacing, 0);
        rangeSlider.Top.Set(y, 0);
        Append(rangeSlider);

        y += (int)rangeSlider.Height.Pixels + Spacing;

        ChannelAmplificationDualKnob = new(0.25f, 3, data.Amplification, data.Channel);
        ChannelAmplificationDualKnob.Left.Set(Spacing, 0);
        ChannelAmplificationDualKnob.Top.Set(y, 0);
        Append(ChannelAmplificationDualKnob);

        RadioButton openMic = new(radioButtons, data.OpenMic, ModAsset.OpenMic.Value);
        openMic.Left.Set(Width.Pixels - Spacing - RadioButton.ButtonWidth, 0);
        openMic.Top.Set(y, 0);
        Append(openMic);

        DeviceSwitcher switcher = new(data.Device);
        switcher.Left.Set(ChannelAmplificationDualKnob.Width.Pixels + (Spacing * 2), 0);
        switcher.Top.Set(y, 0);
        Append(switcher);

        DeviceSwitchButton switcherButton = new(switcher);
        switcherButton.Left.Set(switcher.Left.Pixels + switcher.Width.Pixels + Spacing, 0);
        switcherButton.Top.Set(y, 0);
        Append(switcherButton);

        y += (int)openMic.Height.Pixels + Spacing;

        RadioButton pushToTalk = new(radioButtons, data.PushToTalk, ModAsset.PushToTalk.Value);
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
    }

    private int InitialiseSwitches(UserDataStore data, int y)
    {
        int index = 0;

        AddSwitch(index++, y, ModAsset.Mic.Value, "Mic", data.MicrophoneEnabled);
        AddSwitch(index++, y, ModAsset.Test.Value, "Test", data.TestMode);
        AddSwitch(index++, y, ModAsset.Denoise.Value, "Denoise", data.NoiseSuppression);

        return AddSwitch(index, y, ModAsset.NoIcons.Value, "NoIcons", data.NoIcons);
    }

    private int AddSwitch(int i, int y, Texture2D icon, string label, Ref<bool> setting)
    {
        SwitchButton panelSwitch = new(icon, Language.GetTextValue($"Mods.TerraVoice.UI.{label}"), setting);
        panelSwitch.Left.Set(Spacing + ((Spacing + SwitchButton.SwitchWidth) * i), 0);
        panelSwitch.Top.Set(y, 0);
        Append(panelSwitch);

        return (int)panelSwitch.Height.Pixels;
    }
}
