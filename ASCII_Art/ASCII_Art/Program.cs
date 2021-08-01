using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ASCII_Art
{
    internal static class Program
    {
        private const string Pixels = " .:-=+*#%@";

        private static void Main(string[] args)
        {
            bool isContinue;

            do
            {
                string filePath;
                do
                {
                    Console.Write("Enter file path: ");
                    filePath = Console.ReadLine();
                    if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                        Console.WriteLine("Invalid file path");
                } while (string.IsNullOrEmpty(filePath) || !File.Exists(filePath));

                string asciiWidth;
                do
                {
                    Console.Write("Enter ascii width:");
                    asciiWidth = Console.ReadLine();
                    if (string.IsNullOrEmpty(asciiWidth) || !Regex.IsMatch(asciiWidth, @"^[0-9]+$"))
                        Console.WriteLine("Invalid ascii width");
                } while (string.IsNullOrEmpty(asciiWidth) || !Regex.IsMatch(asciiWidth, @"^[0-9]+$"));


                // Get image bitmap
                var image = new Bitmap(filePath!, true);
                image = GetResizedImage(image, Convert.ToInt32(asciiWidth));

                var result = ConvertToAscii(image);
                Console.WriteLine(result);

                // Is continue or not
                string inpTxt;
                do
                {
                    Console.Write("Continue?(Y/N)");
                    inpTxt = Console.ReadLine();
                } while (string.IsNullOrEmpty(inpTxt) ||
                         !("N".Equals(inpTxt.ToUpper()) || "Y".Equals(inpTxt.ToUpper())));

                isContinue = "Y".Equals(inpTxt.ToUpper());
            } while (isContinue);
        }

        private static string ConvertToAscii(Bitmap image)
        {
            var sb = new StringBuilder();

            for (var h = 0; h < image.Height; h++)
            for (var w = 0; w < image.Width; w++)
            {
                var pixelColor = image.GetPixel(w, h);
                var brightness = Brightness(pixelColor);
                var idx = Math.Round(brightness * (Pixels.Length - 1) / 255);
                sb.Append(Pixels[(int) idx]);

                if (w == image.Width - 1)
                    sb.Append('\n');
            }

            return sb.ToString();
        }

        private static double Brightness(Color c)
        {
            return Math.Sqrt(
                c.R * c.R * .241
                + c.G * c.G * .691
                + c.B * c.B * .068
            );
        }

        private static Bitmap GetResizedImage(Bitmap inpBitmap, int asciiWidth)
        {
            // inpBimap: width -- height
            // ascii: width -- ?
            // ==> asciiHeight = (inpBitmap.Height * asciiWidth) / inpBitmap.Width
            var asciiHeight = (int) Math.Ceiling((double) inpBitmap.Height * asciiWidth / inpBitmap.Width);

            //Create a new Bitmap and define its resolution
            var result = new Bitmap(asciiWidth, asciiHeight);
            var g = Graphics.FromImage(result);
            //The interpolation mode produces high quality images
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(inpBitmap, 0, 0, asciiWidth, asciiHeight);
            g.Dispose();

            return result;
        }
    }
}