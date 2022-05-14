
using StormLibSharp;
using System.Text;

namespace FullClientInstallBuilder
{
    public class Program
    {
        public static readonly Dictionary<string, string> DirectoryPackageLookup = new()
        {
            ["_shaders"] = "Data\\common.MPQ",
            ["Cameras"] = "Data\\patch.MPQ",
            ["Character"] = "Data\\patch.MPQ",
            ["Creature"] = "Data\\lichking.MPQ",
            ["Data"] = "Data\\{ProductLocaleCode}\\base-{ProductLocaleCode}.mpq",
            ["DBFilesClient"] = "Data\\{ProductLocaleCode}\\patch-{ProductLocaleCode}.MPQ",
            ["DUNGEONS"] = "Data\\patch-2.MPQ",
            ["Environments"] = "Data\\patch-2.MPQ",
            ["Fonts"] = "Data\\common.MPQ",
            ["Interface"] = "Data\\{ProductLocaleCode}\\locale-{ProductLocaleCode}.MPQ",
            ["Interiors"] = "Data\\patch-2.MPQ",
            ["ITEM"] = "Data\\patch-3.MPQ",
            ["PARTICLES"] = "Data\\common.MPQ",
            ["shaders"] = "Data\\common.MPQ",
            ["Sound"] = "Data\\{ProductLocaleCode}\\speech-{ProductLocaleCode}.MPQ",
            ["spell"] = "Data\\patch-3.MPQ",
            ["SPELLS"] = "Data\\patch-3.MPQ",
            ["TEST"] = "Data\\patch.MPQ",
            ["textures"] = "Data\\patch-2.MPQ",
            ["TILESET"] = "Data\\patch-2.MPQ",
            ["World"] = "Data\\patch-2.MPQ",
            ["WTF"] = "Data\\common.MPQ",
            ["XTEXTURES"] = "Data\\common.MPQ"
        };

        public static void Main(string[] args)
        {
            Console.WriteLine("Starting up and reading args...");

            var sourceDir = args[0];
            var targetDir = args[1];
            var splitSize = long.Parse(args[2]);

            sourceDir = sourceDir.EndsWith('\\') ? sourceDir : sourceDir + "\\";
            targetDir = targetDir.EndsWith('\\') ? targetDir : targetDir + "\\";

            Console.WriteLine("Finding files...");

            var fileList = new List<FileEntry>();
            BuildFileList(sourceDir.Length, sourceDir, ref fileList);

            Console.WriteLine($"Found {fileList.Count} files.");

            var instructionPath = targetDir + "Instructions.txt";
            if (File.Exists(instructionPath))
                File.Delete(instructionPath);

            var index = 0;
            var sizeWritten = 0L;
            var archiveIndex = 2;
            var archivePrefix = "Installer Tome ";
            var archivePath = targetDir + archivePrefix + archiveIndex + ".mpq";
            Console.WriteLine("Writing new MPQ: " + archivePath);
            if (File.Exists(archivePath))
                File.Delete(archivePath);
            var archive = MpqArchive.CreateNew(archivePath, MpqArchiveVersion.Version1);
            var pass = new MpqPass();
            while (index < fileList.Count)
            {
                var entry = fileList[index++];
                //Console.WriteLine($"Writing {entry}");
                if ((index % 2000) == 0)
                {
                    var progress = Math.Round(((double)index / (double)fileList.Count) * 100d, 4);
                    Console.WriteLine($"Progress: {progress}%");
                }
                pass.AddEntry(entry);
                archive.AddFileFromDiskWithCompression(sourceDir + entry.Path, entry.Path, MpqCompressionTypeFlags.MPQ_COMPRESSION_ZLIB);
                sizeWritten += entry.Size;
                if (((sizeWritten / 1000) / 1000) >= splitSize)
                {
                    archive.Dispose();
                    SaveInstructions(instructionPath, archivePrefix + archiveIndex + ".mpq", pass);
                    sizeWritten = 0L;
                    ++archiveIndex;
                    archivePath = targetDir + archivePrefix + archiveIndex + ".mpq";  
                    if (File.Exists(archivePath))
                        File.Delete(archivePath);
                    archive = MpqArchive.CreateNew(archivePath, MpqArchiveVersion.Version1);
                    pass = new MpqPass();
                    Console.WriteLine("Writing new MPQ: " + archivePath);
                }
            }
            archive.Dispose();
            SaveInstructions(instructionPath, archivePrefix + archiveIndex + ".mpq", pass);

            Console.WriteLine("Done.");
        }

        static void BuildFileList(int rootDirLength, string sourceDir, ref List<FileEntry> fileList)
        {
            foreach (var dir in Directory.EnumerateDirectories(sourceDir))
            {
                BuildFileList(rootDirLength, dir, ref fileList);
            }
            foreach (var file in Directory.EnumerateFiles(sourceDir))
            {
                var size = new FileInfo(file).Length;
                var shortPath = file.Substring(rootDirLength);
                fileList.Add(new FileEntry(shortPath, size));
            }
        }

        static void SaveInstructions(string fileName, string tomeName, MpqPass pass)
        {
            var unmapped = pass.GetUnmappedFiles();
            var mapped = pass.GetMappedDictionary();

            var builder = new StringBuilder();

            if (unmapped.Count > 0)
            {
                var mpqName = "Data\\common.MPQ";
                var text = BuildInstallationDisc(tomeName, mpqName, unmapped);
                builder.Append(text);
            }

            foreach (var group in mapped)
            {
                var key = group.Key;
                var mpqName = "Data\\patch-common.mpq";
                if (DirectoryPackageLookup.TryGetValue(group.Key, out string keyedMpq))
                {
                    mpqName = keyedMpq;
                }
                var text = BuildInstallationDisc(tomeName, mpqName, group.Value);
                builder.Append(text);
            }

            File.AppendAllText(fileName, builder.ToString());
        }

        private static string BuildInstallationDisc(string tomeName, string mpqName, IEnumerable<FileEntry> entries)
        {
            var text = @$"
<disc with_title=""{tomeName}"" with_file=""{tomeName}"" name=""{tomeName}"">
    <archive origin=""disc"" channel=""0"" path=""{tomeName}"">
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

        class MpqPass
        {
            private readonly List<FileEntry> Files = new();

            public void AddEntry(FileEntry entry)
            {
                Files.Add(entry);
            }

            public ILookup<string, FileEntry> GetMappedFiles()
            {
                return Files.Where(entry => entry.Path.Contains('\\'))
                    .ToLookup(entry => entry.Path[..entry.Path.IndexOf('\\')]);
            }

            public Dictionary<string, List<FileEntry>> GetMappedDictionary()
            {
                return Files.Where(entry => entry.Path.Contains('\\'))
                    .GroupBy(entry => entry.Path[..entry.Path.IndexOf('\\')])
                    .ToDictionary(entry => entry.Key, entry => entry.ToList());
            }

            public List<FileEntry> GetUnmappedFiles()
            {
                return Files.Where(Files => !Files.Path.Contains('\\')).ToList();
            }
        }

        class FileEntry
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
}