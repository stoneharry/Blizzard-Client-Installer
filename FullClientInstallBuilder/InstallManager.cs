using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FullClientInstallBuilder
{
    internal class InstallManager
    {
        private long _SplitSize = 2000;
        private string _SourceDir = string.Empty;
        private string _TargetDir = string.Empty;
        private uint _StartIndex = 2;
        private int _MaxThreads = 4;
        private Dictionary<string, string>? _PackageLookup = null;

        public InstallManager SplitSize(long splitSize)
        {
            _SplitSize = splitSize;
            return this;
        }

        public InstallManager TargetDir(string targetDir)
        {
            _TargetDir = targetDir;
            return this;
        }

        public InstallManager SourceDir(string sourceDir)
        {
            _SourceDir = sourceDir;
            return this;
        }

        public InstallManager StartTomeIndex(uint index)
        {
            _StartIndex = index;
            return this;
        }

        public InstallManager NumThreads(int numThreads)
        {
            _MaxThreads = numThreads;
            return this;
        }

        public InstallManager PackageLookup(Dictionary<string, string> lookup)
        {
            _PackageLookup = lookup;
            return this;
        }

        public void RunInstall(List<FileEntry> fileList)
        {
            if (_TargetDir.Length == 0)
                throw new Exception("No target directory set.");
            if (_SourceDir.Length == 0)
                throw new Exception("No source directory set.");
            if (_PackageLookup == null)
                throw new Exception("No package lookup set.");

            var targetDir = _TargetDir;
            var sourceDir = _SourceDir;
            var instructionPath = targetDir + "Instructions.txt";
            if (File.Exists(instructionPath))
                File.Delete(instructionPath);

            var index = 0;
            var sizeWritten = 0L;
            var archiveIndex = _StartIndex;
            var archivePrefix = "Installer Tome ";

            // First build passes
            var passes = new List<MpqPass>();
            var pass = new MpqPass($"{targetDir}{archivePrefix}{archiveIndex}.mpq", sourceDir, $"{archivePrefix}{archiveIndex}.mpq", _PackageLookup);
            while (index < fileList.Count)
            {
                var entry = fileList[index++];
                pass.AddEntry(entry);
                sizeWritten += entry.Size;
                if (((sizeWritten / 1000) / 1000) >= _SplitSize)
                {
                    sizeWritten = 0L;
                    ++archiveIndex;
                    passes.Add(pass);
                    pass = new MpqPass($"{targetDir}{archivePrefix}{archiveIndex}.mpq", sourceDir, $"{archivePrefix}{archiveIndex}.mpq", _PackageLookup);
                }
            }
            passes.Add(pass);

            Console.WriteLine($"Creating {passes.Count} installer tome MPQs with {NumThreads} threads...");

            // Now run each pass as a job
            var tasks = passes.Select(pass => pass.BuildPassWriteTask()).ToList();

            using SemaphoreSlim semaphore = new(_MaxThreads);
            var waitTasks = new List<Task>();
            foreach (var task in tasks)
            {
                semaphore.Wait();
                var waitTask = Task.Factory.StartNew(() =>
                {
                    try
                    {
                        task.RunSynchronously();
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
                waitTasks.Add(waitTask);
            }
            Task.WaitAll(waitTasks.ToArray());
        }
    }
}
