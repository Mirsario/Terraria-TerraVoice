using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TerraVoice.Misc;

public enum TalkMode
{
    /// <summary>
    /// 按住说话
    /// </summary>
    Push,

    /// <summary>
    /// 开关切换
    /// </summary>
    Toggle
}

public class PersonalConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;
    
    [DefaultValue(TalkMode.Push)]
    public TalkMode TalkMode;

    [DefaultValue(true)]
    public bool BgmFadeOut;

    [Slider]
    [Range(0, 200)]
    [DefaultValue(100)]
    public int VoiceVolume;

    [DefaultValue(true)]
    public bool MicrophoneIcon;

    [DefaultValue(false)]
    public bool HearYourself;
    
    public static PersonalConfig Instance;

    public PersonalConfig() {
        Instance = this;
    }
}