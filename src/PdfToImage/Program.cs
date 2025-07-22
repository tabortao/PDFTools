using System;
using System.IO;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        // 假设 PDF 文件放在 Assets 目录下
        string assetsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets");
        if (!Directory.Exists(assetsDir))
        {
            Console.WriteLine($"PDF 文件目录不存在: {assetsDir}");
            return;
        }
        var pdfFiles = Directory.GetFiles(assetsDir, "*.pdf");
        if (pdfFiles.Length == 0)
        {
            Console.WriteLine("未找到 PDF 文件，请将 PDF 放入 Assets 目录。");
            return;
        }


        var options = new PDFToImage.ConvertOptions
        {
            Dpi = 3000, // 建议300以内，过高会非常慢
            ImageFormat = "png", // 可选: "png"/"jpg"/"bmp"
            JpegQuality = 90, // 仅对jpg有效
            OutputDir = "Output" // 输出目录
        };

        PDFToImage.ConvertBatch(pdfFiles, options);
        Console.WriteLine("批量转换完成！");
    }
}
