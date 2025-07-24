using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class AIGenerateClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;

    public AIGenerateClient(string apiUrl, string apiKey = null)
    {
        _apiUrl = apiUrl;
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };
        
        // 添加认证头（如果提供了API密钥）
        if (!string.IsNullOrEmpty(apiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }
    }

    public async Task<string> GenerateAsync(string prompt, string text, string model)
    {
        // 构建请求体JSON
        var requestBody = $"{{\"prompt\":\"{EscapeJsonString(prompt)}\",\"text\":\"{EscapeJsonString(text)}\",\"model\":\"{EscapeJsonString(model)}\"}}";
        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(_apiUrl, content);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return $"HTTP Error ({(int)response.StatusCode}): {errorContent}";
            }
            
            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException httpEx)
        {
            return $"Network Error: {httpEx.Message}";
        }
        catch (Exception ex)
        {
            return $"API Error: {ex.Message}";
        }
    }

    // 转义JSON字符串中的特殊字符
    private string EscapeJsonString(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var sb = new StringBuilder();
        foreach (char c in input)
        {
            switch (c)
            {
                case '\\': sb.Append("\\\\"); break;
                case '\"': sb.Append("\\\""); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                default: sb.Append(c); break;
            }
        }
        return sb.ToString();
    }
}