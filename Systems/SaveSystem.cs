using Terraria.ModLoader;
using TerraVoice.IO;
using TerraVoice.UI;
using TerraVoice.UI.ControlPanel;

namespace TerraVoice.Systems;

internal sealed class UIVoiceInteropSystem : ModSystem
{
    public override void PreSaveAndQuit()
    {
        UserDataStore data = PersistentDataStoreSystem.GetDataStore<UserDataStore>();

        data.ForceSave();
    }
}
