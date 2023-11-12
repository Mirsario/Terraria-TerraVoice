using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace TerraVoice;

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
            if (speakRemainingTime > 0)
                opacity += 0.12f;
            else
                opacity -= 0.14f;

            opacity = Math.Clamp(opacity, 0f, 1f);
        }
    }

    private void FullScreenDrawSpeakingPlayers(Vector2 arg1, float arg2) {
        Main.spriteBatch.Begin(default, default, default, default, default, default, Main.UIScaleMatrix);

        var tex = ModAsset.Speaking;
        int frameCount = _iconAnimationTimer / 10 % 3;
        var frame = tex.Frame(horizontalFrames: 1, verticalFrames: 3, frameX: 0, frameY: frameCount);
        int x = 8;
        int y = 8;
        for (var i = 0; i < Main.maxPlayers; i++) {
            var opacity = _iconOpacity[i];
            if (opacity <= 0) continue;

            var position = new Vector2(x, y);
            Main.spriteBatch.Draw(tex.Value, position, frame, Color.White * opacity, 0f, Vector2.Zero, 1f,
                SpriteEffects.None, 0f);

            position.X += frame.Width + 4;
            DrawPlayerHead(Main.player[i], ref position, opacity, 0.8f, Color.White);;

            y += frame.Height + 4;
        }

        Main.spriteBatch.End();
    }

    private bool DrawSpeakingPlayers() {
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
            Main.spriteBatch.Draw(tex.Value, position, frame, Color.White * opacity, 0f, Vector2.Zero, 1f,
                SpriteEffects.None, 0f);

            position.X += frame.Width + 4;
            DrawPlayerHead(Main.player[i], ref position, opacity, 0.8f, Color.White);

            // var position = player.Center - Main.screenPosition + new Vector2(0, -player.height / 2 - 10);
            // Main.spriteBatch.Draw(frame, position, null, Color.White * opacity, 0f, frame.Size() / 2, 1f,
            //     SpriteEffects.None, 0f);}
        }

        return true;
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
        var layer = new LegacyGameInterfaceLayer("TerraVoice: Speaking Players", DrawSpeakingPlayers, InterfaceScaleType.UI);

        int index = layers.FindIndex(l => l.Name is "Vanilla: Player Chat");
        if (index != -1)
            layers.Insert(index, layer);
    }

    public override void Load()
    {
        Main.OnPostFullscreenMapDraw += FullScreenDrawSpeakingPlayers;
    }

    public override void Unload()
    {
        Main.OnPostFullscreenMapDraw -= FullScreenDrawSpeakingPlayers;
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