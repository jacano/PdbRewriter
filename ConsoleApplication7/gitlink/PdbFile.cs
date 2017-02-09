// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdbFile.cs" company="CatenaLogic">
//   Copyright (c) 2014 - 2016 CatenaLogic. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace GitLink.Pdb
{
    using ConsoleApplication7.gitlink;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    internal class PdbFile : IDisposable
    {
        private readonly BinaryReader _br;
        private readonly FileStream _fs;

        private int _pageByteCount;
        private int _rootByteCount;

        private PdbInfo _info;

        internal PdbFile(string path)
        {
            Path = path;

            _fs = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            _br = new BinaryReader(_fs, Encoding.UTF8, true);

            CheckPdbHeader();
            ReadPdbHeader();
            CheckPdb();
        }

        internal string Path { get; private set; }

        internal int RootPage { get; private set; }

        internal int PagesFree { get; private set; }

        internal int PageCount { get; private set; }

        private void CheckPdbHeader()
        {
            var msf = String.Format("Microsoft C/C++ MSF 7.00\r\n{0}DS\0\0\0", (char)0x1A);
            var bytes = Encoding.UTF8.GetBytes(msf);
            if (!bytes.SequenceEqual(_br.ReadBytes(32)))
            {
                throw new Exception("Pdb header didn't match");
            }
        }

        private void ReadPdbHeader()
        {
            // TODO: Create PdbHeader struct
            //// code here

            _pageByteCount = _br.ReadInt32(); // 0x20
            PagesFree = _br.ReadInt32(); // 0x24 TODO not sure meaning
            PageCount = _br.ReadInt32(); // 0x28 for file
            _rootByteCount = _br.ReadInt32(); // 0x2C
            _br.BaseStream.Position += 4; // 0
            RootPage = _br.ReadInt32(); // 0x34
        }

        private void CheckPdb()
        {
            var length = _fs.Length;
            if (length % _pageByteCount != 0)
            {
                throw new Exception(string.Format(
                    "pdb length {0} bytes per page <> 0, {1}, {2}",
                    length,
                    _pageByteCount,
                    PageCount));
            }

            if (length / _pageByteCount != PageCount)
            {
                throw new Exception(string.Format(
                    "pdb length does not match page count, length: {0}, bytes per page: {1}, page count: {2}",
                    length,
                    _pageByteCount,
                    PageCount));
            }
        }

        private PdbRoot ReadRoot(PdbStream streamRoot)
        {
            var root = new PdbRoot(streamRoot);
            using (var brDirectory = StreamReader(streamRoot))
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

            return root;
        }

        internal PdbStream GetRootPdbStream()
        {
            var pdbStream = new PdbStream();
            pdbStream.ByteCount = _rootByteCount;
            pdbStream.Pages = new int[CountPages(_rootByteCount)];

            GoToPage(RootPage);

            for (var i = 0; i < pdbStream.Pages.Length; i++)
            {
                pdbStream.Pages[i] = _br.ReadInt32();
            }

            return pdbStream;
        }

        private PdbRoot GetRoot()
        {
            var pdbRootStream = GetRootPdbStream();
            return ReadRoot(pdbRootStream);
        }

        #region Reading methods

        private int CountPages(int byteCount)
        {
            return (byteCount + _pageByteCount - 1) / _pageByteCount;
        }

        private void GoToPage(int page)
        {
            _br.BaseStream.Position = page * _pageByteCount;
        }

        private void ReadPage(byte[] bytes, int page, int offset, int count)
        {
            GoToPage(page);

            var read = _br.Read(bytes, offset, count);
            if (read != count)
            {
                throw new Exception(string.Format("tried reading {0} bytes at offset {1}, but only read {2}", count, offset, read));
            }
        }

        private byte[] ReadStreamBytes(PdbStream stream)
        {
            var bytes = new byte[stream.ByteCount];
            var pages = stream.Pages;

            if (pages.Length != 0)
            {
                for (var i = 0; i < pages.Length - 1; i++)
                {
                    ReadPage(bytes, pages[i], i * _pageByteCount, _pageByteCount);
                }

                var j = pages.Length - 1;
                ReadPage(bytes, pages[j], j * _pageByteCount, stream.ByteCount - (j * _pageByteCount));
            }

            return bytes;
        }

        private MemoryStream ReadStream(PdbStream stream)
        {
            return new MemoryStream(ReadStreamBytes(stream));
        }

        private BinaryReader StreamReader(PdbStream stream)
        {
            return new BinaryReader(ReadStream(stream));
        }

        #endregion

        private PdbInfo InternalInfo()
        {
            var info = new PdbInfo();

            var root = GetRoot();
            if (root.Streams.Count <= 1)
            {
                throw new Exception(string.Format(
                    "Expected at least 2 streams inside the pdb root, but only found '{0}', cannot read pdb info",
                    root.Streams.Count));
            }

            using (var ms = new MemoryStream(ReadStreamBytes(root.Streams[1])))
            {
                using (var br = new BinaryReader(ms))
                {
                    info.Version = br.ReadInt32(); // 0x00 of stream
                    info.Signature = br.ReadInt32(); // 0x04
                    info.Age = br.ReadInt32(); // 0x08
                    info.Guid = new Guid(br.ReadBytes(16)); // 0x0C

                    var namesByteCount = br.ReadInt32(); // 0x16
                    var namesByteStart = br.BaseStream.Position; // 0x20
                    br.BaseStream.Position = namesByteStart + namesByteCount;

                    var nameCount = br.ReadInt32();
                    info.FlagIndexMax = br.ReadInt32();
                    info.FlagCount = br.ReadInt32();

                    var flags = new int[info.FlagCount]; // bit flags for each nameCountMax
                    for (var i = 0; i < flags.Length; i++)
                    {
                        flags[i] = br.ReadInt32();
                    }

                    br.BaseStream.Position += 4; // 0
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
                            var position = br.ReadInt32();
                            var stream = br.ReadInt32();
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
                        br.BaseStream.Position = position;

                        var pdbName = new PdbName();
                        pdbName.Name = br.ReadCString();
                        pdbName.Stream = position;

                        info.AddName(pdbName);
                    }

                    return info;
                }
            }
        }

        internal PdbInfo Info
        {
            get { return _info ?? (_info = InternalInfo()); }
        }

        public void Dispose()
        {
            // Move to dispose
            _br.Close();
            _fs.Close();
        }
    }
}