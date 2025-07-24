using System;
using System.IO;
using System.Threading.Tasks;
using AIClient.Logger;
using AIClient.Services;

class Program
{
    static async Task Main(string[] args)
    {
        // 加载配置
        var config = LoadConfiguration();
        
        // 初始化日志系统
        var logLevelStr = config.LogLevel ?? "Information";
        LogLevel logLevel = LogLevel.Information;
        if (Enum.TryParse<LogLevel>(logLevelStr, true, out var parsedLevel))
            logLevel = parsedLevel;
        var logger = new FileLogger(
            config.LogFilePath ?? string.Empty,
            logLevel
        );
        
        logger.Log(LogLevel.Information, "Application started");
        logger.Log(LogLevel.Debug, $"Arguments: {string.Join(" ", args)}");

        try
        {
            // 检查参数
            if (args.Length < 4 || args.Length > 5)
            {
                PrintUsage(logger);
                return;
            }

            string apiUrl = args[0] ?? string.Empty;
            string prompt = args[1] ?? string.Empty;
            string text = args[2] ?? string.Empty;
            string model = args[3] ?? string.Empty;
            string? apiKey = args.Length == 5 ? args[4] : null;

            // 创建客户端
            var client = new AIGenerateClient(
                apiUrl, 
                apiKey ?? string.Empty, 
                logger,
                config.TimeoutSeconds
            );
            
            logger.Log(LogLevel.Information, "=== Request Details ===");
            logger.Log(LogLevel.Information, $"API Endpoint: {apiUrl}");
            logger.Log(LogLevel.Information, $"Prompt: {prompt}");
            logger.Log(LogLevel.Information, $"Text: {text}");
            logger.Log(LogLevel.Information, $"Model: {model}");
            logger.Log(LogLevel.Information, $"Timeout: {config.TimeoutSeconds}s");
            
            if (!string.IsNullOrEmpty(apiKey))
            {
                logger.Log(LogLevel.Information, $"API Key: {AIGenerateClient.MaskApiKey(apiKey)}");
            }
            
            // 发送请求
            logger.Log(LogLevel.Information, "Sending request...");
            var result = await client.GenerateAsync(prompt, text, model);
            
            // 输出结果
            logger.Log(LogLevel.Information, "=== AI Response ===");
            logger.Log(LogLevel.Information, result);
            
            Console.WriteLine("\n=== AI Response ===");
            Console.WriteLine(result);
            Console.WriteLine("===================");
        }
        catch (Exception ex)
        {
            logger.Log(LogLevel.Critical, $"Application Error: {ex.Message}");
            logger.Log(LogLevel.Debug, $"Stack Trace: {ex.StackTrace}");
            
            Console.WriteLine($"\n!!! ERROR !!!");
            Console.WriteLine($"Message: {ex.Message}");
        }
        finally
        {
            logger.Log(LogLevel.Information, "Application exited");
        }
    }

    static void PrintUsage(FileLogger logger)
    {
        logger.Log(LogLevel.Error, "Invalid arguments provided");
        
        Console.WriteLine("AI API Client - .NET 8 Command Line Tool");
        Console.WriteLine("Usage:");
        Console.WriteLine("  Without API Key: AIClient <apiUrl> <prompt> <text> <model>");
        Console.WriteLine("  With API Key:    AIClient <apiUrl> <prompt> <text> <model> <apiKey>");
        Console.WriteLine("\nExample:");
        Console.WriteLine(@"  AIClient https://go.sdgarden.top/api/v1/generate ""Translate to English:"" ""你好世界"" ""goai-chat""");
        Console.WriteLine(@"  AIClient https://go.sdgarden.top/api/v1/generate ""翻译成中文:"" ""Hello world"" ""goai-chat"" sk-1234567890");
    }

    static AppConfig LoadConfiguration()
    {
        // 默认配置
        var config = new AppConfig
        {
            LogFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "AIClient",
                "logs",
                $"log-{DateTime.Now:yyyyMMdd}.txt"
            ),
            LogLevel = "Information",
            TimeoutSeconds = 30
        };

        // 尝试加载appsettings.json
        var settingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        if (File.Exists(settingsPath))
        {
            try
            {
                var json = File.ReadAllText(settingsPath);
                var jsonConfig = System.Text.Json.JsonSerializer.Deserialize<AppConfig>(json);
                if (jsonConfig != null)
                {
                    if (!string.IsNullOrEmpty(jsonConfig.LogFilePath))
                        config.LogFilePath = jsonConfig.LogFilePath;
                    if (!string.IsNullOrEmpty(jsonConfig.LogLevel))
                        config.LogLevel = jsonConfig.LogLevel;
                    if (jsonConfig.TimeoutSeconds > 0)
                        config.TimeoutSeconds = jsonConfig.TimeoutSeconds;
                }
            }
            catch
            {
                // 忽略配置加载错误
            }
        }
        return config;
    }
}

class AppConfig
{
    public string? LogFilePath { get; set; }
    public string? LogLevel { get; set; }
    public int TimeoutSeconds { get; set; }
}