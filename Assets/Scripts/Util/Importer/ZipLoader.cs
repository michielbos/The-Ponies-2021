using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using UnityGLTF.Loader;

namespace Util.Importer {

class ZipLoader : ILoader {
    private readonly ZipArchive zipArchive;

    public ZipLoader(ZipArchive zipArchive) {
        this.zipArchive = zipArchive;
        HasSyncLoadMethod = true;
    }

    public Stream LoadedStream { get; private set; }

    public bool HasSyncLoadMethod { get; }

    public Task LoadStream(string entryName) {
        ZipArchiveEntry entry = zipArchive.GetEntry(entryName);
        if (entry == null) {
            throw new ArgumentNullException("Archive did not contain a " + entryName + " entry.");
        }
        // Apparently the Zip stream has unsupported methods.
        using (Stream stream = entry.Open()) {
            MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return Task.Run(() => LoadedStream = memoryStream);
        }
    }

    public void LoadStreamSync(string entryName) {
        ZipArchiveEntry entry = zipArchive.GetEntry(entryName);
        if (entry == null) {
            throw new ArgumentNullException("Archive did not contain a " + entryName + " entry.");
        }
        // Apparently the Zip stream has unsupported methods.
        using (Stream stream = entry.Open()) {
            MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            LoadedStream = memoryStream;
        }
    }
}

}