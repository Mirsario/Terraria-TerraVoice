using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerraVoice.Core;
using TerraVoice.IO;
using TerraVoice.Misc;

namespace TerraVoice;

public partial class TerraVoice : Mod
{
    private VoiceOutputSystem outputSystem;

    // Sends a voice packet from this client to the server.
    public void PushVoiceBuffer(byte[] buffer) 
    {
        DrawingSystem.PlayerSpeaking[Main.myPlayer] = 20;

        if (Main.netMode != NetmodeID.MultiplayerClient) 
            return;

        byte channel = (byte)PersistentDataStoreSystem.GetDataStore<UserDataStore>().Channel.Value;

        ModPacket packet = Instance.GetPacket();

        packet.Write((byte)0);       // 包类型 - packet ID.
        packet.Write(channel);
        packet.Write(buffer.Length); // 数据大小 - length of the audio data buffer.
        packet.Write(buffer);        // 数据 - the audio data buffer.
        packet.Send();
    }

    public override void PostSetupContent()
        => outputSystem = ModContent.GetInstance<VoiceOutputSystem>();

    // Upon recieving a client voice packet, distribute it to all clients except the sender.
    public override void HandlePacket(BinaryReader reader, int whoAmI) 
    {
        switch (reader.ReadByte()) 
        {
            // 传输中 - audio transmission packet type.
            case 0:
                byte senderChannel = reader.ReadByte();

                int length = reader.ReadInt32();

                byte[] buffer = reader.ReadBytes(length);

                if (Main.netMode is NetmodeID.Server)
                {
                    ModPacket packet = GetPacket();

                    packet.Write((byte)0);
                    packet.Write(senderChannel);
                    packet.Write(length);
                    packet.Write(buffer);
                    packet.Write((byte)whoAmI);

                    packet.Send(ignoreClient: whoAmI);
                }
                else
                {
                    byte sender = reader.ReadByte();

                    byte receiverChannel = (byte)PersistentDataStoreSystem.GetDataStore<UserDataStore>().Channel.Value;

                    // Only process voice data received from players on the same voice channel.
                    if (receiverChannel == senderChannel)
                    {
                        outputSystem.RecieveBuffer(buffer, sender);

                        DrawingSystem.PlayerSpeaking[sender] = 20;
                    }
                }

                break;
        }
    }
}