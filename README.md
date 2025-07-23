# PDFTools类库

![.NET](https://img.shields.io/badge/.NET-8.0%2B-blueviolet)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

一个简单、高效且现代化的 .NET 8 工具，用于将 PDF 文件转换为各种主流图片格式。它包含一个核心类库 (`PDFTools`) 和一个易于使用的命令行工具 (`PDF2ImageTest`)，详见[PDFTools类库说明](./src/PDFTools/README.md)

## API 说明
`PDFTools.Core` 静态类提供了两个核心的异步方法。

### **`PDFToImageAsync`**

将单个 PDF 文档转换为一系列图像文件。

*   **签名**:
    ```csharp
    public static Task<List<string>> PDFToImageAsync(
        string pdfFilePath,
        string outputDirectory,
        int dpi = 150,
        string imageFormat = "jpg",
        int quality = 80)
    ```

*   **参数**:
    *   `pdfFilePath`: 要转换的 PDF 文件路径。
    *   `outputDirectory`: 保存图像文件的目录。
    *   `dpi`: (可选) 输出图像的分辨率。
    *   `imageFormat`: (可选) 输出图像的格式。
    *   `quality`: (可选) 图像质量 (1-100)。

*   **返回**: 一个任务，完成后返回一个包含所有生成图像文件绝对路径的列表。

### **`PDFToImagesAsync`**

将多个 PDF 文档并行转换为图像文件。

*   **签名**:
    ```csharp
    public static Task<List<string>> PDFToImagesAsync(
        IEnumerable<string> pdfFilePaths,
        string outputDirectory,
        int dpi = 150,
        string imageFormat = "jpg",
        int quality = 80,
        int maxConcurrentTasks = 0)
    ```

*   **参数**:
    *   `pdfFilePaths`: 要转换的多个 PDF 文件的路径集合。
    *   `outputDirectory`, `dpi`, `imageFormat`, `quality`: 同上。
    *   `maxConcurrentTasks`: (可选) 最大并行任务数。0 表示使用 `Environment.ProcessorCount` 作为限制。

*   **返回**: 一个任务，完成后返回一个包含所有生成图像文件绝对路径的扁平化列表。
