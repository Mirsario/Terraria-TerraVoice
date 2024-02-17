using Terraria.ModLoader;
using TerraVoice.IO;
using TerraVoice.UI;
using TerraVoice.UI.ControlPanel;

namespace TerraVoice.Systems;

internal sealed class UIVoiceInteropSystem : ModSystem
{
    public override void PostUpdateEverything()
    {
        VoiceControlPanel panel = TerraVoiceUILoader.GetUIState<VoiceControlState>().Panel;

        UserDataStore data = PersistentDataStoreSystem.GetDataStore<UserDataStore>();

        data.MicrophoneEnabled = panel.Switches[VoiceControlPanel.MicrophoneSwitch].Enabled;
        data.TestMode = panel.Switches[VoiceControlPanel.TestSwitch].Enabled;
        data.NoiseSuppression = panel.Switches[VoiceControlPanel.NoiseSuppressionSwitch].Enabled;
        data.NoIcons = panel.Switches[VoiceControlPanel.IconSwitch].Enabled;

        data.Amplification = panel.ChannelAmplificationDualKnob.SmallKnobValue;
        data.Channel = panel.ChannelAmplificationDualKnob.LargeKnobPosition;

        data.PushToTalk = panel.RadioButtons[VoiceControlPanel.PushToTalkRadioButton].Enabled;

        data.ProximitySliderX = panel.RangeSlider.SliderX;
    }

    public override void PreSaveAndQuit()
    {
        UserDataStore data = PersistentDataStoreSystem.GetDataStore<UserDataStore>();

        data.ForceSave();
    }
}
