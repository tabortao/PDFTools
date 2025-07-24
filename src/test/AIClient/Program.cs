using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // 检查参数数量（支持带API密钥和不带API密钥两种格式）
        if (args.Length < 4 || args.Length > 5)
        {
            PrintUsage();
            return;
        }

        string apiUrl = args[0];
        string prompt = args[1];
        string text = args[2];
        string model = args[3];
        string apiKey = args.Length == 5 ? args[4] : null;

        try
        {
            var client = new AIGenerateClient(apiUrl, apiKey);
            Console.WriteLine("=== API Client ===");
            Console.WriteLine($"Endpoint: {apiUrl}");
            Console.WriteLine($"Prompt: {prompt}");
            Console.WriteLine($"Text: {text}");
            Console.WriteLine($"Model: {model}");
            if (!string.IsNullOrEmpty(apiKey)) Console.WriteLine($"API Key: {MaskApiKey(apiKey)}");
            
            Console.WriteLine("\nSending request...");
            var result = await client.GenerateAsync(prompt, text, model);
            
            Console.WriteLine("\n=== AI Response ===");
            Console.WriteLine(result);
            Console.WriteLine("===================");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n!!! ERROR !!!");
            Console.WriteLine($"Message: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
            }
        }
    }

    static void PrintUsage()
    {
        Console.WriteLine("AI API Client - .NET 8 Command Line Tool");
        Console.WriteLine("Usage:");
        Console.WriteLine("  Without API Key: AIClient <apiUrl> <prompt> <text> <model>");
        Console.WriteLine("  With API Key:    AIClient <apiUrl> <prompt> <text> <model> <apiKey>");
        Console.WriteLine("\nExample:");
        Console.WriteLine(@"  AIClient http://localhost:8080/api/v1/generate ""Translate to English:"" ""你好世界"" ""gpt-4o""");
        Console.WriteLine(@"  AIClient http://localhost:8080/api/v1/generate ""翻译成中文:"" ""Hello world"" ""gpt-4o"" sk-1234567890");
    }
    
    static string MaskApiKey(string apiKey)
    {
        if (apiKey.Length <= 8) return new string('*', apiKey.Length);
        return apiKey.Substring(0, 3) + new string('*', apiKey.Length - 6) + apiKey.Substring(apiKey.Length - 3);
    }
}