using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.UI;

namespace TerraVoice.Misc;

[Autoload(Side = ModSide.Client)]
public class DrawingSystem : ModSystem
{
    internal static int[] PlayerSpeaking = new int[Main.maxPlayers];
    private static float[] _iconOpacity = new float[Main.maxPlayers];
    private static int _iconAnimationTimer = 0;

    public override void PostUpdateTime() {
        _iconAnimationTimer++;

        for (var i = 0; i < Main.maxPlayers; i++) {
            ref int speakRemainingTime = ref PlayerSpeaking[i];
            ref float opacity = ref _iconOpacity[i];

            speakRemainingTime--;
            bool speaking = speakRemainingTime > 0;
            if (speaking)
                opacity += 0.12f;
            else
                opacity -= 0.14f;

            opacity = Math.Clamp(opacity, 0f, 1f);
        }
    }

    private void FullscreenMapDraw(Vector2 arg1, float arg2) {
        Main.spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);

        #region Speaking Players

        var tex = ModAsset.Speaking;
        int frameCount = _iconAnimationTimer / 10 % 3;
        var frame = tex.Frame(horizontalFrames: 1, verticalFrames: 3, frameX: 0, frameY: frameCount);
        int x = 8;
        int y = 8;
        for (var i = 0; i < Main.maxPlayers; i++)
        {
            var opacity = _iconOpacity[i];
            if (opacity <= 0) continue;

            var position = new Vector2(x, y);
            var iconColor = i == Main.myPlayer ? Main.OurFavoriteColor : Color.White;
            iconColor *= opacity;
            Main.spriteBatch.Draw(tex.Value, position, frame, iconColor, 0f, Vector2.Zero, 1f, SpriteEffects.None,
                0f);

            position.X += frame.Width + 4;
            DrawPlayerHead(Main.player[i], ref position, opacity, 0.8f, Color.White);
            ;

            y += frame.Height + 4;
        }

        #endregion

        DrawMicrophoneIcon();

        Main.spriteBatch.End();
    }

    private bool DrawSpeakingPlayers() {
        //if (VoiceConfig.Instance.VoiceAttenuation) return true;

        var tex = ModAsset.Speaking;
        int frameCount = _iconAnimationTimer / 10 % 3;
        var frame = tex.Frame(horizontalFrames: 1, verticalFrames: 3, frameX: 0, frameY: frameCount);
        int x = 8;
        int y = Main.screenHeight - 8;
        for (var i = 0; i < Main.maxPlayers; i++) {
            var opacity = _iconOpacity[i];
            if (opacity <= 0) continue;

            y -= frame.Height + 4;
            var position = new Vector2(x, y);
            var iconColor = i == Main.myPlayer ? Main.OurFavoriteColor : Color.White;
            iconColor *= opacity;
            Main.spriteBatch.Draw(tex.Value, position, frame, iconColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            position.X += frame.Width + 4;
            DrawPlayerHead(Main.player[i], ref position, opacity, 0.8f, Color.White);
        }

        return true;
    }

    private bool DrawMicrophoneIcon() {
        /*if (!PersonalConfig.Instance.MicrophoneIcon) return true;
        // 不是按住说话模式，或开启了声音衰减，就会显示图标
        if (!VoiceConfig.Instance.VoiceAttenuation && PersonalConfig.Instance.TalkMode is TalkMode.Push) return true;*/

        var tex = ModAsset.Microphone;
        int frameCount = 0;
        if (SteamUser.GetAvailableVoice(out _) is EVoiceResult.k_EVoiceResultNotRecording)
            frameCount = 1;
        var frame = tex.Frame(horizontalFrames: 1, verticalFrames: 2, frameX: 0, frameY: frameCount);
        var position = Main.ScreenSize.ToVector2() - frame.Size() - new Vector2(6f);
        Main.spriteBatch.Draw(tex.Value, position, frame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        return true;
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
        var speakingPlayersLayer =
            new LegacyGameInterfaceLayer("TerraVoice: Speaking Players", DrawSpeakingPlayers, InterfaceScaleType.UI);
        var microphoneIconLayer =
            new LegacyGameInterfaceLayer("TerraVoice: Microphone Icon", DrawMicrophoneIcon, InterfaceScaleType.UI);

        int index = layers.FindIndex(l => l.Name is "Vanilla: Player Chat");
        if (index != -1)
            layers.InsertRange(index, new[] {speakingPlayersLayer, microphoneIconLayer });
    }

    public override void Load() {
        Main.OnPostFullscreenMapDraw += FullscreenMapDraw;
    }

    public override void Unload() {
        Main.OnPostFullscreenMapDraw -= FullscreenMapDraw;
    }

    public void DrawPlayerHead(Player drawPlayer, ref Vector2 position, float opacity = 1f, float scale = 1f,
        Color borderColor = default) {
        var playerHeadDrawRenderTargetContent = Main.MapPlayerRenderer._playerRenders[drawPlayer.whoAmI];
        playerHeadDrawRenderTargetContent.UsePlayer(drawPlayer);
        playerHeadDrawRenderTargetContent.UseColor(borderColor);
        playerHeadDrawRenderTargetContent.Request();
        Main.MapPlayerRenderer._anyDirty = true;
        Main.MapPlayerRenderer._drawData.Clear();
        if (!playerHeadDrawRenderTargetContent.IsReady) return;

        var target = playerHeadDrawRenderTargetContent.GetTarget();
        var origin = target.Size() / 2f;
        position += new Vector2(11f, 8f);
        Main.MapPlayerRenderer._drawData.Add(new DrawData(target, position, null, Color.White * opacity, 0f,
            origin, scale, SpriteEffects.None));
        Main.MapPlayerRenderer.RenderDrawData(drawPlayer);
    }
}