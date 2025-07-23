# PDFTools

一个简单、高效且现代化的 .NET 8 工具，用于将 PDF 文件转换为各种主流图片格式。它包含一个核心类库 (`PDFTools`) 和一个易于使用的命令行工具 (`PDF2ImageTest`)。

该项目基于强大的[PDFtoImage](https://github.com/sungaila/PDFtoImage) 库，它基于 Google 的 PDFium 渲染引擎，并使用 SkiaSharp 进行图像处理，确保了出色的性能和在 Windows、Linux 及 macOS 上的一致表现，确保了高质量和高性能的转换。

## ✨功能特性

*   **多种图片格式支持**: 支持将 PDF 转换为 **PNG, JPEG, WEBP** 等多种格式。
*   **异步与并行处理**: 利用 `async/await` 和 `SemaphoreSlim`，以非阻塞方式高效处理单个或多个文件，充分利用多核 CPU 性能。
*   **批量转换**: 支持一次性转换指定文件夹内的所有 PDF 文件。
*   **高度可定制**: 用户可以自定义输出图片的 DPI (分辨率) 和质量 (针对 JPEG/WEBP)。
*   **跨平台**: 基于 .NET 8，可以在 Windows, macOS 和 Linux 上无缝运行。
*   **双重接口**:
    *   **命令行工具**: 为最终用户提供简单的命令行界面。
    *   **类库 (API)**: 为开发者提供强大的 API，可轻松集成到任何 .NET 项目中。

## ⚙️系统要求

*   **.NET 8 SDK** (用于构建项目)
*   **.NET 8 Runtime** (仅用于运行已编译的程序)
*   **开发环境 (可选)**: Visual Studio 2022, JetBrains Rider, 或 Visual Studio Code

## 🚀构建项目

您可以轻松地通过 `dotnet` CLI 构建此项目。

1.  **克隆仓库**
    ```bash
    git clone https://github.com/tabortao/PDFTools.git
    cd PDFTools
    ```

2.  **导航到 `src` 目录**
    ```bash
    cd src
    ```

3.  **还原依赖项**
    ```bash
    dotnet restore
    ```

4.  **构建解决方案**
    *   **调试版本**:
        ```bash
        dotnet build
        ```
    *   **发布版本** (优化性能):
        ```bash
        dotnet build -c Release
        ```

    编译后的命令行工具位于 `src/test/PDFToolsTests/bin/<Release|Debug>/net8.0/` 目录下。

## 如何使用 (命令行工具)

命令行工具 `PDF2ImageTest` (或在 Windows 上为 `PDF2ImageTest.exe`) 提供了简单直接的转换方式。

### **命令语法**

```
PDF2ImageTest.exe <输入PDF文件或文件夹> <输出目录> [DPI] [格式] [质量]
```

### **参数说明**

| 参数         | 类型   | 描述                                                               |
| :----------- | :----- | :----------------------------------------------------------------- |
| `<输入...>`  | 必需   | 要转换的单个PDF文件路径，或包含多个PDF文件的文件夹路径。           |
| `<输出目录>` | 必需   | 保存生成图片的文件夹路径。如果不存在，程序会自动创建。             |
| `[DPI]`      | 可选   | 输出图片的分辨率 (每英寸点数)。**默认值: 150**。                    |
| `[格式]`     | 可选   | 输出图片的格式 (png, jpg, webp, bmp, gif)。**默认值: jpg**。       |
| `[质量]`     | 可选   | 图片质量 (1-100)，仅对 `jpg` 和 `webp` 格式有效。**默认值: 80**。 |

### **使用示例**

*   **转换单个 PDF 文件为 JPG (使用默认设置)**
    ```bash
    # Windows
    PDF2ImageTest.exe "C:\MyDocs\report.pdf" "C:\OutputImages"

    # macOS / Linux
    ./PDF2ImageTest "Documents/report.pdf" "OutputImages"
    ```

*   **转换整个文件夹内的所有 PDF 文件**
    ```bash
    # Windows
    PDF2ImageTest.exe "C:\MyPDFFolder" "C:\OutputImages"
    ```

*   **转换文件夹并指定 DPI 和格式为 PNG**
    ```bash
    # Windows
    PDF2ImageTest.exe "C:\MyPDFFolder" "C:\OutputImages" 300 png
    ```

*   **转换单个文件为高质量的 WebP 图片**
    ```bash
    # Windows
    PDF2ImageTest.exe "C:\MyDocs\report.pdf" "C:\OutputImages" 200 webp 95
    ```

---

## API 说明 (类库使用)

开发者可以将 `PDFTools` 类库项目引用到自己的 .NET 应用中，以编程方式执行转换。

### 1. 添加引用

首先，在您的项目中添加对 `PDFTools.csproj` 的项目引用。

### 2. 使用方法

`PDFTools.Core` 静态类提供了两个核心的异步方法。

#### **`PDFToImageAsync`**

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

*   **代码示例**:
    ```csharp
    using PDFTools.Convert;
    using System;
    using System.Threading.Tasks;

    public class MyConverter
    {
        public async Task ConvertSinglePdf()
        {
            try
            {
                var generatedFiles = await Convert.PDFToImageAsync(
                    @"C:\MyDocs\sample.pdf",
                    @"C:\Output"
                );

                Console.WriteLine($"成功生成 {generatedFiles.Count} 个文件:");
                foreach(var file in generatedFiles)
                {
                    Console.WriteLine(file);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"转换失败: {ex.Message}");
            }
        }
    }
    ```

#### **`ToImagesAsync`**

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

*   **代码示例**:
    ```csharp
    using PDFTools.Convert;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class MyBatchConverter
    {
        public async Task ConvertMultiplePdfs()
        {
            var myPdfList = new List<string>
            {
                @"C:\PDFs\report-jan.pdf",
                @"C:\PDFs\report-feb.pdf",
                @"C:\PDFs\presentation.pdf"
            };

            try
            {
                var allGeneratedFiles = await Convert.PDFToImagesAsync(
                    myPdfList,
                    @"C:\BatchOutput",
                    dpi: 200,
                    imageFormat: "png"
                );

                Console.WriteLine($"批量任务完成，总共生成 {allGeneratedFiles.Count} 个图片文件。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"批量转换失败: {ex.Message}");
            }
        }
    }
    ```