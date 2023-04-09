using CliWrap;
using System.IO.Compression;
using System.Text;

namespace SXSEXP_Helper
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("SXSEXPHelper version 1.0.1\n");
            Console.WriteLine("Processing directories and files, please wait...");

            DirectoryInfo Destination;
            string Source = Directory.GetCurrentDirectory();
            DirectoryInfo SFCFixDirectory = Directory.CreateDirectory(@$"{Source}\SFCFix");

            StringBuilder builder = new StringBuilder("::\n");
            builder.AppendLine(@"{ARCHIVE} %systemroot%\WinSxS [DIR]");

            await File.WriteAllTextAsync(@$"{SFCFixDirectory.FullName}\SFCFix.txt", builder.ToString());

            foreach (string directory in Directory.EnumerateDirectories(Source))
            {
                foreach (string path in Directory.EnumerateFiles(directory))
                {
                    string[] segments = path.Split('\\');
                    int length = segments.Length;

                    string file = segments[length - 1];
                    string file_dir = segments[length - 2];

                    DirectoryInfo child_directory = Directory.CreateDirectory($@"{SFCFixDirectory.FullName}\{file_dir}");
                    string[] arguments = new string[] { @$"{path}", $@"{child_directory.FullName}\{file}" };

                    //TODO: Import SXSEXP directly
                    await Cli.Wrap($"{Source}\\sxsexp64.exe").WithArguments(arguments)
                        .WithValidation(CommandResultValidation.None)
                        .ExecuteAsync();
                }
            }

            //Clean up any empty directories
            foreach (string directory_name in Directory.EnumerateDirectories(SFCFixDirectory.FullName))
            {
                DirectoryInfo directory =  Directory.CreateDirectory(directory_name);

                if (directory.GetFiles().Length == 0) directory.Delete();
            }

            ZipFile.CreateFromDirectory(SFCFixDirectory.FullName, @$"{Source}\SFCFix.zip", CompressionLevel.Optimal, false);
            Directory.Delete(SFCFixDirectory.FullName, true);

            Console.WriteLine(@$"The decompressed files have been saved to {Source}\SFCFix.zip");
            Console.Write("Please press any key to exit...");
            Console.ReadKey();
        }
    }
}