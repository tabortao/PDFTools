using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docnet.Core;
using Docnet.Core.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

/// <summary>
/// PDF 批量转图片工具类，提供高效的异步并行转换方法。
/// 基于 Docnet.Core + ImageSharp 跨平台实现。
/// </summary>
public static class PDFToImageConverter
{
    /// <summary>
    /// 异步地将多个 PDF 文件并行转换为图片。
    /// </summary>
    /// <param name="pdfFilePaths">要转换的多个 PDF 文件的路径集合。</param>
    /// <param name="outputDirectory">所有图片的总输出目录。</param>
    /// <param name="dpi">输出图片的分辨率（DPI）。</param>
    /// <param name="imageFormat">输出图片格式（png/jpg/bmp）。</param>
    /// <param name="quality">JPEG 图片质量（1-100，仅对jpg/jpeg有效）。</param>
    /// <param name="maxConcurrentTasks">最大并行任务数。0 表示使用 Environment.ProcessorCount 作为限制。</param>
    /// <returns>一个任务，完成后返回一个包含所有生成图像文件绝对路径的扁平化列表。</returns>
    public static async Task<List<string>> PDFToImagesAsync(
        IEnumerable<string> pdfFilePaths,
        string outputDirectory,
        int dpi = 150,
        string imageFormat = "jpg",
        int quality = 80,
        int maxConcurrentTasks = 0)
    {
        if (pdfFilePaths == null || !pdfFilePaths.Any())
        {
            return new List<string>();
        }

        if (string.IsNullOrWhiteSpace(outputDirectory))
        {
            throw new ArgumentException("Output directory cannot be null or empty.", nameof(outputDirectory));
        }

        Directory.CreateDirectory(outputDirectory);

        var concurrentLimit = maxConcurrentTasks > 0 ? maxConcurrentTasks : Environment.ProcessorCount;
        using var semaphore = new SemaphoreSlim(concurrentLimit);

        var generatedImagePaths = new ConcurrentBag<string>();
        var conversionTasks = new List<Task>();

        foreach (var pdfPath in pdfFilePaths)
        {
            await semaphore.WaitAsync();

            conversionTasks.Add(Task.Run(async () =>
            {
                try
                {
                    var imagePaths = await ConvertPdfAsync(pdfPath, outputDirectory, dpi, imageFormat, quality);
                    foreach (var path in imagePaths)
                    {
                        generatedImagePaths.Add(path);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error] Failed to convert {Path.GetFileName(pdfPath)}: {ex.Message}");
                }
                finally
                {
                    semaphore.Release();
                }
            }));
        }

        await Task.WhenAll(conversionTasks);

        return generatedImagePaths.ToList();
    }

    /// <summary>
    /// 转换单个 PDF 文件的核心实现。
    /// </summary>
    private static Task<List<string>> ConvertPdfAsync(
        string pdfPath,
        string baseOutputDir,
        int dpi,
        string imageFormat,
        int quality)
    {
        if (!File.Exists(pdfPath))
        {
            Console.WriteLine($"[Warning] File not found: {pdfPath}, skipping.");
            return Task.FromResult(new List<string>());
        }

        var imagePaths = new List<string>();
        var fileName = Path.GetFileNameWithoutExtension(pdfPath);
        var fileOutputDir = Path.Combine(baseOutputDir, fileName);
        Directory.CreateDirectory(fileOutputDir);

        try
        {
            // Docnet.Core 本身是同步库，在 Task.Run 中执行可避免阻塞主线程
            using var docReader = DocLib.Instance.GetDocReader(pdfPath, new PageDimensions(dpi, dpi));
            int pageCount = docReader.GetPageCount();

            Console.WriteLine($"-> Converting '{fileName}' ({pageCount} pages)...");

            for (int i = 0; i < pageCount; i++)
            {
                using var pageReader = docReader.GetPageReader(i);
                var rawBytes = pageReader.GetImage(); // BGRA format
                var width = pageReader.GetPageWidth();
                var height = pageReader.GetPageHeight();

                using var image = Image.LoadPixelData<Bgra32>(rawBytes, width, height);
                
                string ext = imageFormat.ToLowerInvariant().Replace("jpeg", "jpg");
                string outPath = Path.Combine(fileOutputDir, $"page_{i + 1}.{ext}");

                switch (ext)
                {
                    case "jpg":
                        var jpegEncoder = new JpegEncoder { Quality = quality };
                        image.Save(outPath, jpegEncoder);
                        break;
                    case "bmp":
                        image.Save(outPath, new BmpEncoder());
                        break;
                    case "png":
                    default:
                        image.Save(outPath, new PngEncoder());
                        break;
                }
                imagePaths.Add(Path.GetFullPath(outPath));
            }
            Console.WriteLine($"   Done -> {fileOutputDir}");
        }
        catch (Exception ex)
        {
            // 包装异常以提供更多上下文
            throw new InvalidOperationException($"Failed to process PDF file '{pdfPath}'. See inner exception for details.", ex);
        }

        return Task.FromResult(imagePaths);
    }
}