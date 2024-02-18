﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using TerraVoice.Core;
using TerraVoice.Native;
using TerraVoice.UI.Abstract;

namespace TerraVoice.UI.ControlPanel;

internal class DeviceSwitcher : SmartUIElement
{
    public string DisplayText => devices.Count == 0 ? Language.GetTextValue($"Mods.TerraVoice.UI.NoInputDevice") : device.Value;

    private const int ScreenWidth = 344;
    private const int ScreenHeight = 32;

    private readonly List<string> devices;

    private readonly Ref<string> device;

    private TextBanner deviceLabel;

    private bool updateBanner;

    public DeviceSwitcher(Ref<string> device)
    {
        devices = ALMono16Microphone.GetDevices();

        this.device = device;

        // If there is a valid audio device connected and the current device is not set, set it to the default audio device.
        if (device.Value == null && devices.Count > 0)
        {
            device.Value = devices[0];
        }

        Width.Set(ScreenWidth, 0);
        Height.Set(ScreenHeight, 0);

        ModContent.GetInstance<VoiceInputSystem>().SwitchAudioDevice(device);

        updateBanner = true;
    }

    public override void SafeUpdate(GameTime gameTime)
    {
        if (updateBanner)
        {
            deviceLabel = new TextBanner(DisplayText, TerraVoice.Font);

            updateBanner = false;
        }

        Rectangle drawBox = GetDimensions().ToRectangle();

        int offset = 2;

        Rectangle shrunkBox = new(drawBox.X + offset, drawBox.Y, drawBox.Width - (offset * 2), drawBox.Height);

        deviceLabel.UpdateScrolling(shrunkBox);
    }

    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        Vector2 position = GetDimensions().Position();
        Rectangle drawBox = GetDimensions().ToRectangle();

        int offset = 2;

        Rectangle shrunkBox = new(drawBox.X + offset, drawBox.Y, drawBox.Width - (offset * 2), drawBox.Height);

        spriteBatch.Draw(ModAsset.DeviceSwitcherWidget.Value, position, Color.White);

        deviceLabel?.Draw(spriteBatch, position + new Vector2(4, 5), shrunkBox, TerraVoice.Cyan);
    }

    public void NextAudioDevice()
    {
        int index = devices.IndexOf(device.Value);

        index++;

        if (index >= devices.Count)
        {
            index = 0;
        }

        device.Value = devices[index];

        ModContent.GetInstance<VoiceInputSystem>().SwitchAudioDevice(device);

        updateBanner = true;
    }
}
