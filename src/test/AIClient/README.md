# AI API Client

AI API Client是一个功能完善的AI API客户端，支持超时设置、多级别日志记录和配置文件管理。

## 主要功能

- **API调用**：支持文本生成API调用
- **超时设置**：可配置请求超时时间（默认30秒）
- **多级别日志**：支持Debug、Information、Warning、Error、Critical五级日志
- **文件日志**：自动记录请求和响应到文件
- **配置文件**：通过appsettings.json配置日志和超时
- **API密钥保护**：自动掩码显示敏感信息

## 项目特点

### 1. 完善的日志系统
- **多级别日志**：支持Debug、Information、Warning、Error、Critical五级日志
- **文件日志**：自动创建日志文件并按日期归档
- **线程安全**：使用锁机制确保并发写入安全
- **异步记录**：支持异步日志写入，减少对主线程影响

### 2. 灵活的配置管理
- **配置文件**：通过appsettings.json管理设置
- **默认路径**：自动选择平台合适的日志路径
- **优先级**：配置文件优先于默认设置

### 3. 健壮的超时处理
- **可配置超时**：默认30秒，可通过配置文件调整
- **超时检测**：专门捕获TaskCanceledException
- **合理默认值**：平衡用户体验和网络波动

### 4. 安全增强
- **API密钥掩码**：自动隐藏敏感信息
- **错误隔离**：单独处理HTTP错误和网络错误
- **异常保护**：确保程序不会因异常而崩溃

### 5. 结构化设计
- **模块化**：分离日志、服务和主程序
- **可扩展**：易于添加新功能（如API版本切换）
- **可维护**：清晰的代码结构和命名规范

### 项目结构
```
test/AIClient/
├── Logger/
│   ├── FileLogger.cs     # 文件日志实现
│   └── LogLevel.cs       # 日志级别枚举
├── Services/
│   └── AIGenerateClient.cs   # AI API客户端实现
├── Program.cs            # 主程序入口
├── appsettings.json     # 应用配置文件
├── AIClient.csproj      # 项目文件
└── README.md            # 项目文档
```

## 安装与使用

### 1. 构建项目
```bash
dotnet build
```

### 2. 运行程序
```bash
# 基本用法（无API密钥）
dotnet run -- <apiUrl> <prompt> <text> <model>

# 带API密钥用法
dotnet run -- <apiUrl> <prompt> <text> <model> <apiKey>
```

### 3. 使用示例
```bash
# 翻译示例
dotnet run -- "http://localhost:8080/api/v1/generate" "请将以下文本翻译成英语：" "你好，我的祖国是中国！" "gpt-40"

# 带API密钥的示例
dotnet run -- "http://localhost:8080/api/v1/generate" "总结以下文本：" "人工智能是未来发展的关键领域" "gpt-40" sk-1234567890
```

## 配置选项

通过`appsettings.json`文件配置：

```json
{
  "LogFilePath": "logs/app-log.txt",  // 日志文件路径
  "LogLevel": "Information",          // 日志级别 (Debug, Information, Warning, Error, Critical)
  "TimeoutSeconds": 45                // 请求超时时间（秒）
}
```

## 日志系统

### 日志级别
- **Debug**：详细调试信息（请求/响应内容）
- **Information**：常规操作信息（默认）
- **Warning**：潜在问题警告
- **Error**：可恢复的错误
- **Critical**：严重错误导致程序终止

### 日志位置
默认日志路径：
- Windows: `%LOCALAPPDATA%\AIClient\logs\log-YYYYMMDD.txt`
- Linux/macOS: `~/.local/share/AIClient/logs/log-YYYYMMDD.txt`

### 日志示例
```
[2023-10-05 14:30:22] [Information] Application started
[2023-10-05 14:30:23] [Information] Sending request to http://localhost:8080/api/v1/generate
[2023-10-05 14:30:25] [Information] Received response: 200 OK
[2023-10-05 14:30:25] [Debug] Response Content: Hello, my motherland is China!
[2023-10-05 14:30:25] [Information] Application exited
```

## 命令行参数

| 参数    | 说明                                  | 示例值                                      |
|---------|---------------------------------------|---------------------------------------------|
| apiUrl  | API端点URL                            | `http://localhost:8080/api/v1/generate`   |
| prompt  | 提示文本                              | `"请将以下文本翻译成英语："`                |
| text    | 需要处理的文本                        | `"你好，世界！"`                            |
| model   | 使用的AI模型名称                      | `"gpt-4o"`                               |
| apiKey  | (可选) API认证密钥                    | `sk-1234567890`                             |

## 错误处理

程序会捕获并记录以下错误类型：
- 网络连接问题
- API请求超时
- HTTP错误响应
- 无效参数
- 程序内部异常

## 超时设置

默认超时为30秒，可通过以下方式调整：
1. 修改appsettings.json中的`TimeoutSeconds`值
2. 在代码中直接修改默认值（高级用户）

## 最佳实践

1. **生产环境**：
   - 设置LogLevel为Information或Warning
   - 定期清理日志文件
   - 使用配置文件管理API密钥（不推荐在命令行传递）

2. **调试环境**：
   - 设置LogLevel为Debug
   - 检查日志文件中的详细请求/响应

3. **敏感信息**：
   - API密钥在日志中自动掩码显示
   - 避免在日志中记录敏感文本内容

## 许可证
MIT License - 自由使用和修改

