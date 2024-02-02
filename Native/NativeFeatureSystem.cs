using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using System.Linq;
using System.Runtime.InteropServices;
using static Terraria.ModLoader.Core.TmodFile;
using System;

namespace TerraVoice.Native;

internal class NativeFeatureSystem : ModSystem
{
    private readonly List<IntPtr> loadedLibs = new();

    public override void Load()
    {
        Terraria.ModLoader.UI.Interface.loadMods.SetLoadStage($"TerraVoice: Installing native libraries...", -1);

        Directory.CreateDirectory(TerraVoice.CachePath);

        CopyLibFromTmod(TerraVoice.CachePath, "librnnoise.dll", out string rnnoise);
        CopyLibFromTmod(TerraVoice.CachePath, "libopus.dll", out string opus);

        loadedLibs.Add(NativeLibrary.Load(rnnoise));
        loadedLibs.Add(NativeLibrary.Load(opus));
    }

    public override void Unload()
    {
        foreach (IntPtr handle in loadedLibs)
        {
            NativeLibrary.Free(handle);
        }
    }

    private void CopyLibFromTmod(string cachePath, string lib, out string finalPath)
    {
        finalPath = Path.Combine(cachePath, lib);

        if (File.Exists(finalPath))
            return;

        IDictionary<string, FileEntry> files = typeof(TmodFile)
            .GetField("files", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(Mod.File) as IDictionary<string, FileEntry>;

        FileEntry rnnoise = files.First(f => f.Key == $"lib/{lib}").Value;

        using Stream stream = Mod.File.GetStream(rnnoise);

        byte[] bytes = new byte[rnnoise.Length];

        stream.Read(bytes, 0, bytes.Length);

        File.WriteAllBytes(finalPath, bytes);

        Mod.Logger.Info($"{lib} successfully copied to {cachePath}.");
    }
}
