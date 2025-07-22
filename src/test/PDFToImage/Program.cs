using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length < 2)
        {
            ShowHelp();
            return;
        }

        string inputPath = args[0];
        string outputDirectory = args[1];

        // --- 解析可选参数 ---
        int dpi = 150;
        string imageFormat = "jpg";
        int quality = 80;
        int maxConcurrentTasks = 0; // 0 表示自动

        try
        {
            for (int i = 2; i < args.Length; i += 2)
            {
                string flag = args[i].ToLower();
                if (i + 1 >= args.Length) {
                    Console.WriteLine($"[Error] Missing value for flag {flag}");
                    return;
                }
                string value = args[i + 1];

                switch (flag)
                {
                    case "--dpi":
                        dpi = int.Parse(value);
                        break;
                    case "--format":
                        imageFormat = value.ToLower();
                        break;
                    case "--quality":
                        quality = int.Parse(value);
                        break;
                    case "--concurrency":
                        maxConcurrentTasks = int.Parse(value);
                        break;
                    default:
                        Console.WriteLine($"[Warning] Unknown flag: {flag}");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] Invalid optional arguments: {ex.Message}");
            ShowHelp();
            return;
        }


        // --- 获取要转换的文件列表 ---
        List<string> pdfFiles;
        if (Directory.Exists(inputPath))
        {
            pdfFiles = Directory.GetFiles(inputPath, "*.pdf").ToList();
            Console.WriteLine($"Found {pdfFiles.Count} PDF files in directory: {inputPath}");
        }
        else if (File.Exists(inputPath) && inputPath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            pdfFiles = new List<string> { inputPath };
            Console.WriteLine($"Found single PDF file: {inputPath}");
        }
        else
        {
            Console.WriteLine($"[Error] Input path is not a valid file or directory: {inputPath}");
            return;
        }

        if (!pdfFiles.Any())
        {
            Console.WriteLine("No PDF files to convert.");
            return;
        }
        
        // --- 执行转换 ---
        var stopwatch = Stopwatch.StartNew();
        Console.WriteLine("\nStarting conversion process...");
        Console.WriteLine($"Parameters: DPI={dpi}, Format={imageFormat}, Quality={quality}, MaxConcurrency={ (maxConcurrentTasks == 0 ? "Auto" : maxConcurrentTasks) }");
        Console.WriteLine("-------------------------------------------------");


        try
        {
            List<string> generatedImages = await PDFToImageConverter.PDFToImagesAsync(
                pdfFilePaths: pdfFiles,
                outputDirectory: outputDirectory,
                dpi: dpi,
                imageFormat: imageFormat,
                quality: quality,
                maxConcurrentTasks: maxConcurrentTasks
            );

            stopwatch.Stop();
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("Conversion finished!");
            Console.WriteLine($"Successfully generated {generatedImages.Count} image(s).");
            Console.WriteLine($"Total time taken: {stopwatch.Elapsed.TotalSeconds:F2} seconds.");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"\n[FATAL] An unhandled error occurred: {ex.Message}");
        }
    }

    static void ShowHelp()
    {
        Console.WriteLine("\nPDF to Image Converter");
        Console.WriteLine("------------------------");
        Console.WriteLine("Usage: PDFConverter.exe <input_path> <output_directory> [options]");
        Console.WriteLine("\nArguments:");
        Console.WriteLine("  <input_path>          Path to a single PDF file or a directory containing PDF files.");
        Console.WriteLine("  <output_directory>    Directory where the output images will be saved.");
        
        Console.WriteLine("\nOptions:");
        Console.WriteLine("  --dpi <number>        Set the resolution in Dots Per Inch (DPI). Default: 150.");
        Console.WriteLine("  --format <type>       Set the output image format (png, jpg, bmp). Default: jpg.");
        Console.WriteLine("  --quality <1-100>     Set the quality for JPG images. Default: 80.");
        Console.WriteLine("  --concurrency <num>   Set the max number of parallel tasks. Default: auto (processor count).");
        
        Console.WriteLine("\nExamples:");
        Console.WriteLine("  // Convert a single file");
        Console.WriteLine("  PDFConverter.exe C:\\Docs\\report.pdf C:\\Output\\Images");
        
        Console.WriteLine("\n  // Convert all PDFs in a folder with custom settings");
        Console.WriteLine("  PDFConverter.exe C:\\All_PDFs C:\\Output --dpi 200 --format png");
    }
}