using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullClientInstallBuilder
{
    internal class FileEntry
    {
        public readonly string Path;
        public readonly long Size;

        public FileEntry(string path, long size)
        {
            Path = path;
            Size = size;
        }

        public string ConvertEntryToInstruction()
        {
            return $"<repack input0=\"{Path}\" to=\"{Path}\" size=\"{Size}\" />";
        }

        public override string ToString()
        {
            return $"FileEntry[{Path}, {Size}]";
        }
    }
}
