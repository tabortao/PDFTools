# PdfToImage

一个基于 Docnet.Core + ImageSharp 的跨平台 PDF 批量转图片工具，支持自定义分辨率（DPI）、图片格式（png/jpg/bmp）、输出目录、自动分文件夹归档。

## 功能特性
- 支持批量将 PDF 文件转换为图片（png/jpg/bmp）
- 可自定义输出图片分辨率（DPI）和 JPEG 质量
- 输出图片自动按 PDF 文件名分文件夹归档
- 支持自定义输出目录
- 跨平台（Windows、Linux、macOS）

## 环境要求
- .NET 8.0 SDK 或更高版本
- 推荐使用 VS Code 或 Visual Studio 2022+

## 依赖包
- [Docnet.Core](https://github.com/GowenGit/docnet)
- [SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp)

## 快速开始

### 1. 克隆项目
```bash
# 进入你的工作目录
cd your-workspace
# 克隆代码
# git clone <your-repo-url>
```

### 2. 安装依赖
```bash
cd PdfToImage
# 安装 Docnet.Core 和 ImageSharp
 dotnet add package Docnet.Core
 dotnet add package SixLabors.ImageSharp
```

### 3. 添加 PDF 文件
将待转换的 PDF 文件放入 `Assets` 目录（如没有请新建）。

### 4. 编译项目
```bash
dotnet build
```

### 5. 运行项目
```bash
dotnet publish -c Release -r win-x64 --self-contained false # 编译为可执行文件
dotnet run
```

### 6. 查看输出
- 转换后的图片会自动输出到 `Output/原PDF文件名/` 目录下。
- 支持 png、jpg、bmp 格式，图片分辨率和质量可在 `Program.cs` 里配置。

## 配置说明
在 `Program.cs` 中可自定义如下参数：
```csharp
var options = new PDFToImage.ConvertOptions
{
    Dpi = 300, // 建议300以内，过高会非常慢
    ImageFormat = "png", // 可选: "png"/"jpg"/"bmp"
    JpegQuality = 90, // 仅对jpg有效
    OutputDir = "Output" // 输出目录
};
```

## 开发说明
- 主要逻辑在 `PDFToImage.cs`，调用 Docnet.Core 读取 PDF，ImageSharp 生成图片。
- 支持跨平台，无需 Python 环境。
- 如需自定义图片处理，可参考 ImageSharp 官方文档。

## 常见问题
- **DPI 设置过高很慢/内存占用大？**
  建议 300~600，特殊需求可调高。
- **图片模糊？**
  可能是 PDF 源文件分辨率低，或渲染引擎低 DPI 下字体细节损失。
- **如何支持更多图片格式？**
  可参考 ImageSharp 文档扩展。

## 许可证
MIT License
