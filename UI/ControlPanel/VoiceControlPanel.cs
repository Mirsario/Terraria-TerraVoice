using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using TerraVoice.UI.Abstract;

namespace TerraVoice.UI.ControlPanel;

internal class VoiceControlPanel : SmartUIElement
{
    private const int PanelWidth = 640;
    private const int PanelHeight = 448;

    private float oldScale;

    private readonly VoiceControlState parent;

    public VoiceControlPanel(VoiceControlState parent)
    {
        this.parent = parent;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);

        Vector2 boxPosition = GetDimensions().Position();

        spriteBatch.Draw(ModAsset.MainPanel.Value, boxPosition, Color.White);

        if (!Main.gamePaused)
        {
            if (Main.UIScale != oldScale)
            {
                parent.Recalculate();

                oldScale = Main.UIScale;
            }
        }
    }

    public override void Recalculate()
    {
        Width.Set(PanelWidth, 0);
        Height.Set(PanelHeight, 0);
        Left.Set(-Width.Pixels / 2, 0.5f);
        Top.Set(-Height.Pixels / 2, 0.5f);

        base.Recalculate();
    }
}
