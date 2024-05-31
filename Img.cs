using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace image_to_ascii
{
    internal class Img
    {
        public Bitmap Bmp { get; }

        public Img(string path)
        {
            try {
                Image picture = Image.FromFile(path);
                Bmp = ResizeImage(picture, 900);
            } catch (ArgumentException) {
                Console.WriteLine("Please enter a valid input path");
            } catch (FileNotFoundException) {
                Console.WriteLine("Input file not found");
            } catch (Exception) {
                Console.WriteLine("Something went wrong with the input");
            }
        }

        private Bitmap ResizeImage(Image image, int max)
        {
            // Aspect Ratio
            double aspectRatio = (double)image.Width / (double)image.Height;
            string biggestDimension = image.Width > image.Height ? "width" : "height";
            int width; int height;
            if (biggestDimension == "width") {
                width = max;
                height = (int)(width / aspectRatio);
            } else if (biggestDimension == "height") {
                height = max;
                width = (int)(height * aspectRatio);
            } else {
                Console.WriteLine("Image resizing failed");
                return null;
            }

            // Resizing
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage)) {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes()) {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public void ToAscii(string asciiPath) 
        {
            var builder = new StringBuilder();

            if (Bmp == null) { return; }

            for (int y = 0; y < Bmp.Height; y++) {
                for (int x = 0; x < Bmp.Width; x++) {
                    Color pixelColor = Bmp.GetPixel(x, y);
                    double rgbAverage = AverageRgbValue(pixelColor);

                    char symbol;
                    if (rgbAverage >= 0 && rgbAverage < 75) {
                        symbol = ' ';
                    } else if (rgbAverage >= 75 && rgbAverage < 105) {
                        symbol = '\'';
                    } else if (rgbAverage >= 105 && rgbAverage < 120) {
                        symbol = '"';
                    } else if (rgbAverage >= 120 && rgbAverage < 150) {
                        symbol = '*';
                    } else if (rgbAverage >= 150 && rgbAverage < 155) {
                        symbol = '|';
                    } else if (rgbAverage >= 155 && rgbAverage < 165) {
                        symbol = '/';
                    } else if (rgbAverage >= 175 && rgbAverage < 180) {
                        symbol = '{';
                    } else if (rgbAverage >= 180 && rgbAverage < 185) {
                        symbol = '[';
                    } else if (rgbAverage >= 185 && rgbAverage < 200) {
                        symbol = '(';
                    } else if (rgbAverage >= 200 && rgbAverage < 210) {
                        symbol = '$';
                    } else if (rgbAverage >= 210 && rgbAverage < 225) {
                        symbol = '%';
                    } else if (rgbAverage >= 235 && rgbAverage < 250) {
                        symbol = '#';
                    } else {
                        symbol = '@';
                    }

                    builder.Append(symbol);
                }
                builder.Append('\n');
            }
            string ascii = builder.ToString();
            if (!File.Exists(asciiPath)) {
                try {
                    File.WriteAllText(asciiPath, ascii);
                } catch (DirectoryNotFoundException) {
                    Console.WriteLine("Output directory not found");
                    return;
                } catch (PathTooLongException) {
                    Console.WriteLine("Output path too long");
                    return;
                } catch (UnauthorizedAccessException) {
                    Console.WriteLine("Unauthorized access to output directory");
                    return;
                } catch (Exception) {
                    Console.WriteLine("Something went wrong with the output");
                    return;
                }
                
            } else {
                Console.WriteLine("Cannot name new file the same as a existing file");
                return;
            }

            if (File.Exists(asciiPath)) {
                Console.WriteLine($"\nSuccessfully created a file: \"{asciiPath}\"");
            } else {
                Console.WriteLine($"\nFailed in creating a file: \"{asciiPath}\"");
            }      
        }

        private double AverageRgbValue(Color colors)
        {
            int[] rgb = new int[3];

            rgb[0] = colors.R;
            rgb[1] = colors.G;
            rgb[2] = colors.B;

            return rgb.Average();
        }
    }
}
