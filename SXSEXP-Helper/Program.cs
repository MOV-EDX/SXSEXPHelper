﻿using CliWrap;
using System.IO.Compression;
using System.Text;

namespace SXSEXP_Helper
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("SXSEXPHelper version 1.1.1\n");
            Console.WriteLine("Processing directories and files, please wait...");

            string Source = Directory.GetCurrentDirectory();
            DirectoryInfo SFCFixDirectory = Directory.CreateDirectory(@$"{Source}\SFCFix");

            if (File.Exists(@$"{Source}\SFCFix.zip")) File.Delete(@$"{Source}\SFCFix.zip");

            StringBuilder builder = new StringBuilder("::\n");
            builder.AppendLine(@"{ARCHIVE}\WinSxS %systemroot%\WinSxS [DIR]");
            builder.AppendLine(@"{ARCHIVE}\Manifests %systemroot%\WinSxS\Manifests [DIR]");

            await File.WriteAllTextAsync(@$"{SFCFixDirectory.FullName}\SFCFix.txt", builder.ToString());

            foreach (string directory in Directory.EnumerateDirectories(Source).Except(new string[] { SFCFixDirectory.FullName }))
            {
                foreach (string path in Directory.EnumerateFiles(directory))
                {
                    string[] segments = path.Split('\\');
                    int length = segments.Length;

                    string file = segments[length - 1];
                    string file_dir = segments[length - 2];

                    string child_dir = directory.Contains("Manifests") ? "Manifests" : "WinSxS";

                    DirectoryInfo child_directory = directory.Contains("Manifests") ? Directory.CreateDirectory($@"{SFCFixDirectory.FullName}\{child_dir}") : Directory.CreateDirectory($@"{SFCFixDirectory.FullName}\{child_dir}\{file_dir}");
                    string[] arguments = new string[] { @$"{path}", $@"{child_directory.FullName}\{file}" };

                    //TODO: Import SXSEXP directly
                    await Cli.Wrap($"{Source}\\sxsexp64.exe").WithArguments(arguments)
                        .WithValidation(CommandResultValidation.None)
                        .ExecuteAsync();
                }
            }

            //Clean up any empty directories
            foreach (string directory_name in Directory.EnumerateDirectories(SFCFixDirectory.FullName, "*", SearchOption.AllDirectories))
            {
                //Gets file handle to directory object
                DirectoryInfo directory = Directory.CreateDirectory(directory_name);

                if (directory.GetFiles("*", SearchOption.AllDirectories).Length == 0) directory.Delete();
            }

            ZipFile.CreateFromDirectory(SFCFixDirectory.FullName, @$"{Source}\SFCFix.zip", CompressionLevel.Optimal, false);
            Directory.Delete(SFCFixDirectory.FullName, true);

            Console.WriteLine(@$"The decompressed files have been saved to {Source}\SFCFix.zip");
            Console.Write("Please press any key to exit...");
            Console.ReadKey();
        }
    }
}