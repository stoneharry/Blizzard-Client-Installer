

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
            var splitSize = args.Length > 2 ? long.Parse(args[2]) : 2000L;
            var numThreads = args.Length > 3 ? int.Parse(args[3]) : 4;

            sourceDir = sourceDir.EndsWith('\\') ? sourceDir : sourceDir + "\\";
            targetDir = targetDir.EndsWith('\\') ? targetDir : targetDir + "\\";

            Console.WriteLine("Finding files...");

            var fileList = new List<FileEntry>();
            BuildFileList(sourceDir.Length, sourceDir, ref fileList);

            Console.WriteLine($"Found {fileList.Count} files.");

            new InstallManager()
                .SourceDir(sourceDir)
                .TargetDir(targetDir)
                .SplitSize(splitSize)
                .StartTomeIndex(2)
                .PackageLookup(DirectoryPackageLookup)
                .NumThreads(numThreads)
                .RunInstall(fileList);

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
    }
}