namespace GitLink.Pdb
{
    using ConsoleApplication7.gitlink;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class PdbFile
    {
        private const string FileIndicator = "/src/files/";

        private int _pageByteCount;
        private int _rootByteCount;
        private int _pageCount;
        private int _pagesFree;
        private int _rootPage;

        private PdbRoot _root;

        public PdbFile(string path)
        {
            Path = path;

            using (var fs = File.Open(path, FileMode.Open))
            {
                using (var br = new BinaryReader(fs))
                {
                    CheckPdbHeader(br);
                    ReadPdbHeader(br);
                    CheckPdb(fs);

                    _root = ReadRoot(fs, br);
                }
            }
        }

        public string Path { get; private set; }

        public PdbInfo Deserialize()
        {
            using (var fs = File.Open(Path, FileMode.Open))
            {
                using (var br = new BinaryReader(fs))
                {
                    return InternalInfo(fs, br);
                }
            }
        }

        private void CheckPdbHeader(BinaryReader br)
        {
            var msf = string.Format("Microsoft C/C++ MSF 7.00\r\n{0}DS\0\0\0", (char)0x1A);
            var bytes = Encoding.ASCII.GetBytes(msf);
            if (!bytes.SequenceEqual(br.ReadBytes(32)))
            {
                throw new Exception("Pdb header didn't match");
            }
        }

        private void ReadPdbHeader(BinaryReader br)
        {
            // TODO: Create PdbHeader struct
            //// code here

            _pageByteCount = br.ReadInt32(); // 0x20
            _pagesFree = br.ReadInt32(); // 0x24 TODO not sure meaning
            _pageCount = br.ReadInt32(); // 0x28 for file
            _rootByteCount = br.ReadInt32(); // 0x2C
            var dummy = br.ReadInt32();  // 0
            _rootPage = br.ReadInt32(); // 0x34
        }

        private void CheckPdb(FileStream fs)
        {
            var length = fs.Length;
            if (length % _pageByteCount != 0)
            {
                throw new Exception(string.Format(
                    "pdb length {0} bytes per page <> 0, {1}, {2}",
                    length,
                    _pageByteCount,
                    _pageCount));
            }

            if (length / _pageByteCount != _pageCount)
            {
                throw new Exception(string.Format(
                    "pdb length does not match page count, length: {0}, bytes per page: {1}, page count: {2}",
                    length,
                    _pageByteCount,
                    _pageCount));
            }
        }

        private PdbRoot ReadRoot(FileStream fs, BinaryReader br)
        {
            var pdbStreamRoot = GetRootPdbStream(fs, br);
            var root = new PdbRoot(pdbStreamRoot);
            using (var ms = new MemoryStream(ReadStreamBytes(fs, br, pdbStreamRoot)))
            {
                using (var brDirectory = new BinaryReader(ms))
                {
                    var streamCount = brDirectory.ReadInt32();
                    if (streamCount != 0x0131CA0B)
                    {
                        var streams = root.Streams;
                        for (var i = 0; i < streamCount; i++)
                        {
                            var stream = new PdbStream();
                            streams.Add(stream);

                            var byteCount = brDirectory.ReadInt32();
                            stream.ByteCount = byteCount;

                            var pageCount = CountPages(byteCount);
                            stream.Pages = new int[pageCount];
                        }

                        for (var i = 0; i < streamCount; i++)
                        {
                            for (var j = 0; j < streams[i].Pages.Length; j++)
                            {
                                var page = brDirectory.ReadInt32();
                                streams[i].Pages[j] = page;
                            }
                        }
                    }
                }
            }

            return root;
        }

        private PdbStream GetRootPdbStream(FileStream fs, BinaryReader br)
        {
            var pdbStream = new PdbStream();
            pdbStream.ByteCount = _rootByteCount;
            pdbStream.Pages = new int[CountPages(_rootByteCount)];

            GoToPage(fs, br, _rootPage);

            for (var i = 0; i < pdbStream.Pages.Length; i++)
            {
                pdbStream.Pages[i] = br.ReadInt32();
            }

            return pdbStream;
        }

        private int CountPages(int byteCount)
        {
            return (byteCount + _pageByteCount - 1) / _pageByteCount;
        }

        private void GoToPage(FileStream fs, BinaryReader br, int page)
        {
            var numBytes = page * _pageByteCount;
            fs.Seek(numBytes, SeekOrigin.Begin);
        }

        private void ReadPage(FileStream fs, BinaryReader br, byte[] bytes, int page, int offset, int count)
        {
            GoToPage(fs, br, page);

            var read = br.Read(bytes, offset, count);
            if (read != count)
            {
                throw new Exception(string.Format("tried reading {0} bytes at offset {1}, but only read {2}", count, offset, read));
            }
        }

        private byte[] ReadStreamBytes(FileStream fs, BinaryReader br, PdbStream stream)
        {
            var bytes = new byte[stream.ByteCount];
            var pages = stream.Pages;

            if (pages.Length != 0)
            {
                for (var i = 0; i < pages.Length - 1; i++)
                {
                    ReadPage(fs, br, bytes, pages[i], i * _pageByteCount, _pageByteCount);
                }

                var j = pages.Length - 1;
                ReadPage(fs, br, bytes, pages[j], j * _pageByteCount, stream.ByteCount - (j * _pageByteCount));
            }

            return bytes;
        }

        private PdbInfo InternalInfo(FileStream fs, BinaryReader br)
        {
            var info = new PdbInfo();

            if (_root.Streams.Count <= 1)
            {
                throw new Exception(string.Format(
                    "Expected at least 2 streams inside the pdb root, but only found '{0}', cannot read pdb info",
                    _root.Streams.Count));
            }

            using (var ms = new MemoryStream(ReadStreamBytes(fs, br, _root.Streams[1])))
            {
                using (var brData = new BinaryReader(ms))
                {
                    info.Version = brData.ReadInt32(); // 0x00 of stream
                    info.Signature = brData.ReadInt32(); // 0x04
                    info.Age = brData.ReadInt32(); // 0x08
                    info.Guid = new Guid(brData.ReadBytes(16)); // 0x0C

                    var namesByteCount = brData.ReadInt32(); // 0x16
                    var namesByteStart = ms.Position; // 0x20

                    var offset = namesByteStart + namesByteCount;
                    ms.Seek(offset, SeekOrigin.Begin);

                    var nameCount = brData.ReadInt32();
                    info.FlagIndexMax = brData.ReadInt32();
                    info.FlagCount = brData.ReadInt32();

                    var flags = new int[info.FlagCount]; // bit flags for each nameCountMax
                    for (var i = 0; i < flags.Length; i++)
                    {
                        flags[i] = brData.ReadInt32();
                    }

                    var dummy = brData.ReadInt32(); // 0

                    var positions = new List<int>(nameCount);
                    for (var i = 0; i < info.FlagIndexMax; i++)
                    {
                        var flagIndex = i / 32;
                        if (flagIndex >= flags.Length)
                        {
                            break;
                        }

                        var flag = flags[flagIndex];
                        if ((flag & (1 << (i % 32))) != 0)
                        {
                            var position = brData.ReadInt32();
                            var stream = brData.ReadInt32();
                            positions.Add(position);
                        }
                    }

                    if (positions.Count != nameCount)
                    {
                        throw new Exception(string.Format("names count, {0} <> {1}", positions.Count, nameCount));
                    }

                    foreach (var index in positions)
                    {
                        var position = namesByteStart + index;

                        ms.Seek(position, SeekOrigin.Begin);

                        var globalPosition = fs.Position;
                        var name = brData.ReadCString();

                        if (name.StartsWith(FileIndicator))
                        {
                            var nameFinal = name.Substring(FileIndicator.Length);

                            var pdbName = new PdbName();
                            pdbName.Name = nameFinal;
                            pdbName.Stream = globalPosition;

                            info.AddName(pdbName);
                        }
                    }

                    return info;
                }
            }
        }
    }
}