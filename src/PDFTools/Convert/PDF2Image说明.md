
namespace PDFTools.Convert
实现ToImages类，包含构造函数和具有如下参数：


private static async Task<List<string>> ToIamge

- pdfFilePath：要转换的 PDF 文件路径
- outputDirectory：保存图像文件的目录
- imageFormat：输出图像的格式 （例如 png、jpeg、webp，默认为jpeg）
- 




```c#
namespace PDFTools.Convert
{
    /// <summary>
    /// PDF2Image is a tool for converting PDF files to images.
    /// It provides functionality to render PDF pages as images with various options.
    /// </summary>
    public static class Convert
    {
        // --- Internal helper to convert a single PDF file to images ---
        // This method encapsulates the core logic for processing one PDF file.
        // It's private because the public API is the static ToImages and ToImage.
        public static async Task<List<string>> ToIamge
        {
            // Add methods for converting PDF to images here.
            // For example, you can implement methods like ToImages, GetPageCount, etc.
        }   

    }
}
```