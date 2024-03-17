//Written for ASURA. https://store.steampowered.com/app/2293900/
using System.IO.Compression;

FileStream input = File.OpenRead(args[0]);
BinaryReader br = new(input);
Directory.CreateDirectory(Path.GetDirectoryName(args[0]) + "\\" + Path.GetFileNameWithoutExtension(args[0]));
if (new string(br.ReadChars(7)) != "D-PACK2")
    throw new Exception("Not a ASURA dat file.");

br.ReadByte();
int fileCount = br.ReadInt32();
int size = br.ReadInt32();

List<SUBFILE> subfiles = [];
for (int i = 0; i < fileCount; i++)
{
    subfiles.Add(new()
    {
        unknown = br.ReadSingle(),
        start = br.ReadInt32(),
        sizeUncompressed = br.ReadInt32(),
        sizeCompressed = br.ReadInt32(),
        name = i
    });
}

foreach (SUBFILE subfile in subfiles)
{
    br.BaseStream.Position = subfile.start;

    using FileStream FS = File.Create(Path.GetDirectoryName(args[0]) + "\\" + Path.GetFileNameWithoutExtension(args[0]) + "\\" + subfile.name);
    BinaryWriter bw = new(FS);

    MemoryStream ms = new();
    br.ReadInt16();
    using (var ds = new DeflateStream(new MemoryStream(br.ReadBytes(subfile.sizeCompressed - 2)), CompressionMode.Decompress))
        ds.CopyTo(ms);
    br = new(ms);
    br.BaseStream.Position = 0;

    bw.Write(br.ReadBytes(subfile.sizeUncompressed));
    bw.Close();
    br = new(input);
}

class SUBFILE
{
    public float unknown;
    public int start;
    public int sizeUncompressed;
    public int sizeCompressed;
    public int name;
}