using Terraria.ModLoader;
using TerraVoice.Core;
using TerraVoice.UI;
using TerraVoice.UI.ControlPanel;

namespace TerraVoice.Systems;

internal sealed class UIVoiceInteropSystem : ModSystem
{
    public override void PostUpdateEverything()
    {
        VoiceControlPanel panel = TerraVoiceUILoader.GetUIState<VoiceControlState>().Panel;

        VoiceInputSystem inputSystem = ModContent.GetInstance<VoiceInputSystem>();
        VoiceProcessingSystem processingSystem = ModContent.GetInstance<VoiceProcessingSystem>();
        IconDrawingSystem iconDrawingSystem = ModContent.GetInstance<IconDrawingSystem>();

        inputSystem.MicrophoneEnabled = panel.Switches[VoiceControlPanel.MicrophoneSwitch].Enabled;
        processingSystem.TestMode = panel.Switches[VoiceControlPanel.TestSwitch].Enabled;
        processingSystem.NoiseSuppression = panel.Switches[VoiceControlPanel.NoiseSuppressionSwitch].Enabled;
        iconDrawingSystem.NoIcons = panel.Switches[VoiceControlPanel.IconSwitch].Enabled;

        processingSystem.Amplification = panel.Amplification.Volume;
        processingSystem.Threshold = panel.Threshold.Volume;
        processingSystem.PushToTalk = panel.RadioButtons[VoiceControlPanel.PushToTalkRadioButton].Enabled;
    }
}
