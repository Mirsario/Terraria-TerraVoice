using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerraVoice.Core;
using TerraVoice.Misc;

namespace TerraVoice;

public partial class TerraVoice : Mod
{
    public static void PushVoiceBuffer(byte[] buffer, uint dataSize) {
        DrawingSystem.PlayerSpeaking[Main.myPlayer] = 20;

        if (Main.netMode is not NetmodeID.MultiplayerClient) return;
        var p = Instance.GetPacket();
        p.Write((byte) 0); // 包类型
        p.Write(dataSize); // 数据大小
        p.Write(buffer.Take((int) dataSize).ToArray()); // 数据
        p.Send();
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI) {
        switch (reader.ReadByte()) {
            case 0: // 传输中
                uint dataSize = reader.ReadUInt32();
                var buffer = reader.ReadBytes((int) dataSize);
                if (Main.netMode is NetmodeID.Server) {
                    var p = GetPacket();
                    p.Write((byte) 0);
                    p.Write(dataSize);
                    p.Write(buffer);
                    p.Write((byte) whoAmI);
                    p.Send(ignoreClient: whoAmI);
                }
                else {
                    byte sender = reader.ReadByte();
                    PlayVoiceSystem.ReceiveVoiceBuffer(sender, buffer, dataSize);
                    DrawingSystem.PlayerSpeaking[sender] = 20;
                }

                break;
        }
    }
}