using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TerraVoice.UI.Abstract;

namespace TerraVoice.UI.ControlPanel;

internal class AudioVisualiserWidget : SmartUIElement
{
    public override void DrawSelf(SpriteBatch spriteBatch)
    {
        Vector2 position = GetDimensions().Position();

        spriteBatch.Draw(ModAsset.AudioVisualiserWidget.Value, position, Color.White);

        base.DrawSelf(spriteBatch);
    }
}
