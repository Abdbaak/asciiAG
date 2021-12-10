using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace asciiAG
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("A path/url to an image was not specified.");
                Console.WriteLine("Usage: dotnet run \"<Path with quotations>\"");
                Console.WriteLine("Usage: dotnet run -w \"<URL with quotations>\"");
                Console.Write("Press any key to continue...");
                Console.ReadKey(true);
                Environment.Exit(0);
            }
            if (args[0].ToLower() == "-w")
            {
                Console.WriteLine("Web mode enabled...");
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "py.exe";
                string script = "downloader.py";
                string url = args[1];
                psi.Arguments = $"\"{script}\" \"{url}\"";

                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;

                string result = "";
                using (var process = Process.Start(psi))
                {
                    result = process.StandardOutput.ReadToEnd();
                    Console.WriteLine(result);
                }
                string s = BitmapToString(result);
                File.WriteAllTextAsync("output.txt", s, System.Text.Encoding.UTF8);
            }
            else
            {
                string s = BitmapToString(args[0]);
                File.WriteAllTextAsync("output.txt", s, System.Text.Encoding.UTF8);
            }
            Console.WriteLine("Done! To eliminate the stretch, use Notepad++\nInstall Notepad++: https://notepad-plus-plus.org/downloads/");
            Console.Write("Press any key to continue...");
            Console.ReadKey(true);
        }

        static string BitmapToString(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("The specified path does not exist.");
                Environment.Exit(0);
            }
            string output = "";
            Bitmap original = (Bitmap)Image.FromFile(path);
            /* 
                Most text editors will vertically stretch the text, so I am deliberately horizontally stretching
                it beforehand to retain the correct aspect ratio.
            */
            Bitmap bmp = new Bitmap(original, new Size((int)Math.Round(original.Width / 1.5), (int)Math.Round(original.Height / 3.0)));
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color px = bmp.GetPixel(x, y);
                    output += LuminanceToChar(px);
                }
                output += "\n";
            }
            return output;
        }

        static string LuminanceToChar(Color pixelColour)
        {
            string[] asciiChars =
            {
                " ", ".", "'", "`", "^", "\"", ",", ":", ";", "I", "l", "!", "i",
                ">", "<", "~", "+", "_", "-", "?", "]", "[", "}", "{", "1", ")",
                "(", "|", "\\", "/", "t", "f", "j", "r", "x", "n", "u", "v", "c",
                "z", "X", "Y", "U", "J", "C", "L", "Q", "0", "O", "Z", "m", "w",
                "q", "p", "d", "b", "k", "h", "a", "o", "*", "#", "M", "W", "&",
                "8", "%", "B", "@", "$"
            };
            Array.Reverse(asciiChars); // Lower luminance values will use darker characters, for black text on white background editors.
            var charIndex = (int)Math.Floor(
                (0.2126 * pixelColour.R + 0.7152 * pixelColour.G + 0.0722 * pixelColour.B) / 3.6
            );
            if (charIndex > 0)
            {
                charIndex -= 1;
            }
            return asciiChars[charIndex].ToString();
        }

    }
}