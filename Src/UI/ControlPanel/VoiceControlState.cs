using System.Collections.Generic;
using Terraria.UI;
using TerraVoice.UI.Abstract;

namespace TerraVoice.UI.ControlPanel;

internal class VoiceControlState : SmartUIState
{
    public VoiceControlPanel Panel { get; private set; }

    public override int InsertionIndex(List<GameInterfaceLayer> layers) => layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));

    public override void OnInitialize()
    {
        Panel = new(this);

        Append(Panel);
    }
}
