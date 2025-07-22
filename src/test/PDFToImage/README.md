# PDF to Image Converter CLI

![.NET](https://img.shields.io/badge/.NET-8.0%2B-blueviolet)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

这是一个基于 .NET 的高性能、跨平台命令行工具，用于将 PDF 文件批量转换为图片（JPG, PNG, BMP）。它利用并行处理来最大化多核 CPU 的性能，并提供了丰富的自定义选项。

该工具基于 [Docnet.Core](https://github.com/GowenGit/docnet) 进行 PDF 渲染和 [SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp) 进行图像处理。

## 核心功能

*   **灵活的输入**: 支持转换单个 PDF 文件或整个目录中的所有 PDF 文件。
*   **高性能**: 异步与并行处理机制，可同时转换多个 PDF 文件，显著缩短处理时间。
*   **高度可定制**:
    *   自定义输出图片的分辨率 (DPI)。
    *   选择输出格式 (JPG, PNG, BMP)。
    *   为 JPG 格式设置特定的压缩质量。
    *   手动控制最大并发任务数。
*   **结构化输出**: 为每个输入的 PDF 文件自动创建独立的子目录，用于存放其生成的图片页面。
*   **跨平台**: 可在 Windows, macOS 和 Linux 上编译和运行。

## 系统要求

*   [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) 或更高版本。

## 安装与构建

1.  **克隆仓库**

    ```bash
    git clone https://github.com/tabortao/PDFTools.git
    cd src\test\PDFToImage
    ```

2.  **安装依赖**

    该项目依赖于两个主要的 NuGet 包。如果您是首次构建，.NET会自动还原它们。如果需要手动安装，请运行：

    ```bash
    dotnet add package Docnet.Core
    dotnet add package SixLabors.ImageSharp
    ```

3.  **构建可执行文件**

    使用以下命令编译项目。我们推荐使用 `Release` 配置以获得最佳性能。

    ```bash
    dotnet build -c Release
    ```

    编译成功后，您可以在 `bin/Release/net8.0/` (或类似) 目录下找到可执行文件 (`PDFToImage.exe` on Windows, `PDFToImage` on Linux/macOS)。

## 使用方法

### 命令语法

```bash
PDFToImage.exe <input_path> <output_directory> [options]
```

### 参数详解

| 参数                 | 类型   | 描述                                                                          |
| -------------------- | ------ | ----------------------------------------------------------------------------- |
| `input_path`         | **必需** | 源路径。可以是一个 PDF 文件的完整路径，或是一个包含多个 PDF 文件的目录路径。 |
| `output_directory`   | **必需** | 输出根目录。转换后的图片将保存在此目录下，并为每个 PDF 创建一个同名子目录。   |

### 可选选项

| 选项               | 格式              | 描述                                                              | 默认值          |
| ------------------ | ----------------- | ----------------------------------------------------------------- | --------------- |
| `--dpi`            | `<number>`        | 设置输出图片的分辨率 (Dots Per Inch)。数值越高，图片越清晰，文件也越大。 | `150`           |
| `--format`         | `<type>`          | 设置输出图片的格式。可用值：`png`, `jpg` (或 `jpeg`), `bmp`。       | `jpg`           |
| `--quality`        | `<1-100>`         | 当格式为 `jpg` 时有效，用于设置图片压缩质量。                        | `80`            |
| `--concurrency`    | `<number>`        | 设置最大并行任务数（同时转换的 PDF 文件数）。设为 `0` 表示自动（等于CPU核心数）。 | `0` (自动)        |

---

## 使用示例

#### 示例 1：转换单个 PDF 文件

将一个名为 `annual-report-2024.pdf` 的文件转换为图片，并保存到 `D:\Converted\` 目录，使用所有默认设置。

**命令:**

```bash
PDFToImage.exe "C:\Documents\annual-report-2024.pdf" "D:\Converted"
```

**输出结构:**

```
D:\Converted\
└── annual-report-2024\
    ├── page_1.jpg
    ├── page_2.jpg
    └── ...
```

---

#### 示例 2：转换整个目录中的 PDF

转换 `C:\My-PDF-Collection\` 目录下的所有 PDF 文件，并指定输出为 200 DPI 的 PNG 图片。

**命令:**

```bash
PDFToImage.exe "C:\My-PDF-Collection" "C:\Output\Images" --dpi 200 --format png
```

**假设 `My-PDF-Collection` 目录包含 `fileA.pdf` 和 `fileB.pdf`，输出结构如下:**

```
C:\Output\Images\
├── fileA\
│   ├── page_1.png
│   └── page_2.png
└── fileB\
    ├── page_1.png
    ├── page_2.png
    └── page_3.png
```

---

#### 示例 3：高级用法（自定义所有参数）

转换 `C:\PDFs` 目录下的所有 PDF 文件，要求输出 300 DPI、质量为 95 的 JPG 图片，并限制最多同时处理 4 个文件以避免系统负载过高。

**命令:**

```bash
PDFToImage.exe "C:\PDFs" "D:\HighQuality-JPGs" --dpi 300 --format jpg --quality 95 --concurrency 4
```

---

## 技术实现

*   **并行控制**: 使用 `System.Threading.SemaphoreSlim` 来精确控制并发运行的任务数量，防止一次性启动过多 I/O 和 CPU 密集型任务，从而保持系统稳定。
*   **异步执行**: 每个 PDF 文件的转换操作都通过 `Task.Run` 被分派到线程池中执行。这确保了主线程（UI或控制台）不会被阻塞，并能充分利用多核处理器的优势。
*   **线程安全集合**: 使用 `System.Collections.Concurrent.ConcurrentBag<T>` 来安全地从多个并行任务中收集所有生成的图片文件路径。

## 依赖库

*   [**Docnet.Core**](https://github.com/GowenGit/docnet): 一个高效的 .NET PDF 库包装器，用于渲染 PDF 页面为原始像素数据。
*   [**SixLabors.ImageSharp**](https://github.com/SixLabors/ImageSharp): 一个功能强大的、跨平台的 2D 图形库，用于将原始像素数据编码为 JPG, PNG, BMP 等标准图片格式。

## 许可证

该项目根据 [MIT 许可证](https://opensource.org/licenses/MIT) 授权。