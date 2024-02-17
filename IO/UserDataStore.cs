using Terraria.ModLoader.IO;

namespace TerraVoice.IO;

internal class UserDataStore : PersistentDataStore
{
    public override string FileName => "voice_settings.dat";

    // I would normally use properties here, but they cannot be used with the ref keyword.
    public bool MicrophoneEnabled;
    public bool TestMode;
    public bool NoiseSuppression;
    public bool NoIcons;
    public bool PushToTalk = true;

    public float Amplification = 1;

    public int ProximitySliderX;
    public int Channel;

    public string Device;

    public override void LoadGlobal(TagCompound tag)
    {
        LoadTag(tag, nameof(MicrophoneEnabled), ref MicrophoneEnabled);
        LoadTag(tag, nameof(TestMode), ref TestMode);
        LoadTag(tag, nameof(NoiseSuppression), ref NoiseSuppression);
        LoadTag(tag, nameof(NoIcons), ref NoIcons);
        LoadTag(tag, nameof(PushToTalk), ref PushToTalk);

        LoadTag(tag, nameof(Amplification), ref Amplification);

        LoadTag(tag, nameof(ProximitySliderX), ref ProximitySliderX);
        LoadTag(tag, nameof(Channel), ref Channel);

        LoadTag(tag, nameof(Device), ref Device);
    }

    public override void SaveGlobal(TagCompound tag)
    {
        tag[nameof(MicrophoneEnabled)] = MicrophoneEnabled;
        tag[nameof(TestMode)] = TestMode;
        tag[nameof(NoiseSuppression)] = NoiseSuppression;
        tag[nameof(NoIcons)] = NoIcons;
        tag[nameof(PushToTalk)] = PushToTalk;

        tag[nameof(Amplification)] = Amplification;

        tag[nameof(ProximitySliderX)] = ProximitySliderX;
        tag[nameof(Channel)] = Channel;

        tag[nameof(Device)] = Device;
    }

    private void LoadTag<T>(TagCompound tag, string name, ref T property)
    {
        if (tag.ContainsKey(name))
        {
            property = tag.Get<T>(name);
        }
    }
}
