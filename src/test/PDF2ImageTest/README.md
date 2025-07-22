## 构建和运行程序

1.  **构建整个解决方案**
    这个命令会编译所有项目并确保没有错误。

    ```bash
    dotnet build
    ```
    如果一切顺利，你应该会看到 "Build succeeded" 的消息。

2.  **运行程序**
    要运行程序并向其传递参数，我们使用 `dotnet run` 命令。

    *   `--project PDF2ImageTest`: 告诉 `dotnet` 我们要运行哪个项目。
    *   `--`: 这个双破折号非常重要。它用于分隔 `dotnet` 命令自身的参数和我们要传递给**我们自己的应用程序**的参数。

    **准备测试文件**:
    *   在 `D:\` 盘创建一个名为 `Test` 的文件夹。
    *   在 `D:\Test` 里，再创建两个文件夹：`Inputs` 和 `Outputs`。
    *   将一些PDF文件（比如 `sample.pdf`）放入 `Inputs` 文件夹中。

    **运行示例**:
    (请在 `PDF2ImageTest` 根目录下运行以下命令)

    *   **显示帮助信息 (不带参数):**
        ```bash
        dotnet run --project PDF2ImageTest
        # dotnet run --project PDF2ImageTest # 在Solution目录下运行
        ```

    *   **转换单个文件 (使用默认设置):**
        ```bash
        dotnet run -- "D:\Test\Inputs\sample.pdf" "D:\Test\Outputs"
        ```

    *   **转换整个文件夹 (使用自定义设置: 300 DPI, png格式):**
        ```bash
        dotnet run -- "D:\Test\Inputs" "D:\Test\Outputs" 300 png
        ```

执行命令后，你将会在终端看到程序的输出，并且转换后的图片文件会出现在 `D:\Test\Outputs` 文件夹中。