using Terraria.ModLoader;
using TerraVoice.UI;
using TerraVoice.UI.ControlPanel;

namespace TerraVoice.IO;

internal sealed class UIVoiceInteropSystem : ModSystem
{
    public override void PreSaveAndQuit()
    {
        UserDataStore data = PersistentDataStoreSystem.GetDataStore<UserDataStore>();

        data.ForceSave();
    }
}
