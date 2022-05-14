using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StormLibSharp;

namespace FullClientInstallBuilder
{
    internal class MpqPass
    {
        private readonly List<FileEntry> Files = new();
        private readonly string _ArchivePath;
        private readonly string _SourceDir;
        private readonly string _TomeName;
        private readonly Dictionary<string, string> _PackageLookup;
        private static readonly Object _Lock = new();

        public MpqPass(string archivePath, string sourceDir, string tomeName, Dictionary<string, string> packageLookup)
        {
            _ArchivePath = archivePath;
            _SourceDir = sourceDir;
            _PackageLookup = packageLookup;
            _TomeName = tomeName;
        }

        public void AddEntry(FileEntry entry) => Files.Add(entry);

        public List<FileEntry> GetUnmappedFiles() => Files
            .Where(Files => !Files.Path.Contains('\\')).ToList();

        public ILookup<string, FileEntry> GetMappedFiles() => Files
            .Where(entry => entry.Path.Contains('\\'))
            .ToLookup(entry => entry.Path[..entry.Path.IndexOf('\\')]);

        public Dictionary<string, List<FileEntry>> GetMappedDictionary() => Files
            .Where(entry => entry.Path.Contains('\\'))
            .GroupBy(entry => entry.Path[..entry.Path.IndexOf('\\')])
            .ToDictionary(entry => entry.Key, entry => entry.ToList());

        public Task BuildPassWriteTask()
        {
            return new Task(() =>
            {
                Console.WriteLine("Writing new MPQ: " + _ArchivePath);
                if (File.Exists(_ArchivePath))
                    File.Delete(_ArchivePath);
                using (var archive = MpqArchive.CreateNew(_ArchivePath, MpqArchiveVersion.Version1))
                {
                    int lastState = -1;
                    for (var index = 0; index < Files.Count; ++index)
                    {
                        var entry = Files[index];
                        if ((index % 100) == 0)
                        {
                            var progress = (int)Math.Round(((double)index / (double)Files.Count) * 100d);
                            if (progress != lastState)
                            {
                                lastState = progress;
                                Console.WriteLine($"[{_TomeName}]: {progress}%");
                            }
                        }
                        archive.AddFileFromDiskWithCompression(_SourceDir + entry.Path, entry.Path, MpqCompressionTypeFlags.MPQ_COMPRESSION_ZLIB);
                    }
                }
                SaveInstructions();
            });
        }

        private void SaveInstructions()
        {
            var unmapped = GetUnmappedFiles();
            var mapped = GetMappedDictionary();

            var builder = new StringBuilder();

            if (unmapped.Count > 0)
            {
                var mpqName = "Data\\common.MPQ";
                var text = BuildInstallationDisc(mpqName, unmapped);
                builder.Append(text);
            }

            foreach (var group in mapped)
            {
                var mpqName = "Data\\patch-common.mpq";
                if (_PackageLookup.TryGetValue(group.Key, out string? keyedMpq))
                {
                    mpqName = keyedMpq;
                }
                var text = BuildInstallationDisc(mpqName, group.Value);
                builder.Append(text);
            }

            lock (_Lock)
            {
                File.AppendAllText("Instructions.txt", builder.ToString());
            }
        }

        private string BuildInstallationDisc(string mpqName, IEnumerable<FileEntry> entries)
        {
            var text = @$"
<disc with_title=""{_TomeName}"" with_file=""{_TomeName}"" name=""{_TomeName}"">
    <archive origin=""disc"" channel=""0"" path=""{_TomeName}"">
        <target location=""user"">
            <repack_into type=""mpq"" options=""type_and_creator/W!pqWoW!"" container=""{mpqName}"">
{string.Join('\n', entries.Select(entry => entry.ConvertEntryToInstruction()))}
            </repack_into>
        </target>
    </archive>
</disc>
            ";
            return text.Replace("&", "&amp;");
        }
    }
}
