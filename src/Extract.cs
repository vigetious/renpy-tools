using System;
using System.IO;
using static script_reader.Command;


namespace script_reader {
    public class Extract {
        static string config = Directory.GetCurrentDirectory() + "/config";
        public FileInfo[] rpyFiles { get; set; }
        public Extract(FileInfo[] folder) {
            Console.WriteLine("Creating temporary extraction folder...");
            DirectoryInfo tempExtractionFolder = Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/temp");
            Console.WriteLine("Extracting all .rpa files...");
            foreach (var rpaFile in folder) {
                ExtractRpa(rpaFile.FullName, "temp");
            }
            FileInfo[] rpaFiles = tempExtractionFolder.GetFiles("*.rpa", SearchOption.AllDirectories);
            Console.WriteLine("Extracted root .rpa. Checking for other .rpa files contained in the root file...");
            for (int i = 0; i < rpaFiles.Length; i++) {
                ExtractRpa(rpaFiles[i].FullName, "temp");
            }
            Console.WriteLine("Extracted all sub .rpa files. Decompiling .rpyc files...");
            UnixCommand("python2", $"config/unrpyc/unrpyc.py ./temp/");
            foreach (var rpycFile in tempExtractionFolder.GetFiles("*.rpyc", SearchOption.AllDirectories)) {
                rpycFile.Delete();
            }
            Console.WriteLine("Decompiled .rpyc files.");
            rpyFiles = tempExtractionFolder.GetFiles("*.rpy", SearchOption.AllDirectories);
        }

        static void ExtractRpa(string folder, string extractionLocation) {
            UnixCommand($"{config}/venv/bin/python3", $"-m unrpa \"{folder}\" -p {extractionLocation}/");
        }

        static void ExtractRpyc() {
            UnixCommand("python2", $"config/unrpyc/unrpyc.py ./temp/");
        }
    }
}