// 文件: PDF2ImageTest/Program.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// 使用我们创建的类库的命名空间
using PDFTools.Core;

namespace PDF2ImageTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // 检查参数数量是否足够
            if (args.Length < 2)
            {
                ShowHelp();
                return;
            }

            // --- 参数解析 ---
            string inputPath = args[0];
            string outputDirectory = args[1];
            int dpi = 150;
            string format = "jpg";
            int quality = 80;

            // 解析可选参数
            if (args.Length > 2) int.TryParse(args[2], out dpi);
            if (args.Length > 3) format = args[3].ToLowerInvariant();
            if (args.Length > 4) int.TryParse(args[4], out quality);


            // --- 输入文件处理 ---
            List<string> pdfFilePaths = new List<string>();

            if (File.Exists(inputPath))
            {
                // 输入是单个文件
                if (Path.GetExtension(inputPath).ToLowerInvariant() == ".pdf")
                {
                    pdfFilePaths.Add(inputPath);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("错误: 指定的输入文件不是一个PDF文件。");
                    Console.ResetColor();
                    return;
                }
            }
            else if (Directory.Exists(inputPath))
            {
                // 输入是一个文件夹
                pdfFilePaths = Directory.GetFiles(inputPath, "*.pdf", SearchOption.TopDirectoryOnly).ToList();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"错误: 输入路径 '{inputPath}' 不存在。");
                Console.ResetColor();
                return;
            }

            if (!pdfFilePaths.Any())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("在指定路径中没有找到任何PDF文件。");
                Console.ResetColor();
                return;
            }


            // --- 执行转换 ---
            try
            {
                Console.WriteLine($"发现 {pdfFilePaths.Count} 个PDF文件。准备开始转换...");
                Console.WriteLine($"输出目录: {outputDirectory}");
                Console.WriteLine($"设置: DPI={dpi}, 格式={format}, 质量={quality}");
                Console.WriteLine("-------------------------------------------------");
                
                var stopwatch = Stopwatch.StartNew();

                int totalGeneratedFiles = 0;

                foreach (var pdfFilePath in pdfFilePaths)
                {
                    string pdfFileName = Path.GetFileNameWithoutExtension(pdfFilePath);
                    string pdfOutputDirectory = Path.Combine(outputDirectory, pdfFileName);

                    Directory.CreateDirectory(pdfOutputDirectory);

                    Console.WriteLine($"正在转换: {pdfFileName}");

                    var generatedFiles = await PDFTools.Core.Convert.PDFToImagesAsync(
                        new List<string> { pdfFilePath },
                        pdfOutputDirectory,
                        dpi: dpi,
                        imageFormat: format,
                        quality: quality
                    );

                    totalGeneratedFiles += generatedFiles.Count;
                }

                stopwatch.Stop();
                
                Console.WriteLine("-------------------------------------------------");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("转换成功！");
                Console.ResetColor();
                Console.WriteLine($"处理了 {pdfFilePaths.Count} 个PDF文件，生成了 {totalGeneratedFiles} 个图像文件。");
                Console.WriteLine($"总耗时: {stopwatch.Elapsed.TotalSeconds:F2} 秒。");

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n在转换过程中发生错误:");
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("PDF 转图片工具");
            Console.WriteLine("--------------------------");
            Console.WriteLine("用法: ");
            Console.WriteLine("  PDF2ImageTest.exe <输入PDF文件或文件夹> <输出目录> [DPI] [格式] [质量]");
            Console.WriteLine();
            Console.WriteLine("参数:");
            Console.WriteLine("  <输入...>   (必需) 要转换的单个PDF文件路径，或包含多个PDF文件的文件夹路径。");
            Console.WriteLine("  <输出目录>  (必需) 保存图片的文件夹路径。");
            Console.WriteLine("  [DPI]       (可选) 分辨率, 默认 150。");
            Console.WriteLine("  [格式]      (可选) png, jpg, webp, bmp, gif。默认 jpg。");
            Console.WriteLine("  [质量]      (可选) 1-100, 仅对jpg/webp有效。默认 80。");
            Console.WriteLine();
            Console.WriteLine("示例:");
            Console.WriteLine(@"  // 转换单个文件");
            Console.WriteLine(@"  PDF2ImageTest.exe ""C:\MyDocs\report.pdf"" ""C:\MyImages""");
            Console.WriteLine();
            Console.WriteLine(@"  // 转换整个文件夹中的所有PDF，并指定DPI和格式");
            Console.WriteLine(@"  PDF2ImageTest.exe ""C:\MyPDFFolder"" ""C:\MyImages"" 200 png");
        }
    }
}