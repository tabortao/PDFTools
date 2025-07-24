using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AIClient.Logger;

namespace AIClient.Services
{
    public class AIGenerateClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly FileLogger _logger;
        private readonly int _timeoutSeconds;

        public AIGenerateClient(string apiUrl, string? apiKey, FileLogger logger, int timeoutSeconds = 30)
        {
            _apiUrl = apiUrl;
            _logger = logger;
            _timeoutSeconds = timeoutSeconds;

            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(_timeoutSeconds)
            };

            // 添加认证头
            if (!string.IsNullOrEmpty(apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            }

            _logger.Log(LogLevel.Information, $"Initialized API client for {apiUrl}");
            _logger.Log(LogLevel.Debug, $"Timeout: {_timeoutSeconds}s, API Key: {MaskApiKey(apiKey)}");
        }

        public async Task<string> GenerateAsync(string prompt, string text, string model)
        {
            // 构建请求体JSON
            var requestBody = $"{{\"prompt\":\"{EscapeJsonString(prompt)}\",\"text\":\"{EscapeJsonString(text)}\",\"model\":\"{EscapeJsonString(model)}\"}}";

            // 记录请求
            _logger.Log(LogLevel.Information, $"Sending request to {_apiUrl}");
            _logger.Log(LogLevel.Debug, $"Request Body: {requestBody}");

            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(_apiUrl, content);

                // 记录响应状态
                _logger.Log(LogLevel.Information, $"Received response: {(int)response.StatusCode} {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.Log(LogLevel.Error, $"HTTP Error ({(int)response.StatusCode}): {errorContent}");
                    return $"HTTP Error ({(int)response.StatusCode}): {errorContent}";
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                // 记录响应内容
                _logger.Log(LogLevel.Information, $"Response received ({responseContent.Length} chars)");
                _logger.Log(LogLevel.Debug, $"Response Content: {responseContent}");

                return responseContent;
            }
            catch (TaskCanceledException)
            {
                var errorMessage = $"Request timed out after {_timeoutSeconds} seconds";
                _logger.Log(LogLevel.Error, errorMessage);
                return errorMessage;
            }
            catch (HttpRequestException httpEx)
            {
                _logger.Log(LogLevel.Error, $"Network Error: {httpEx.Message}");
                return $"Network Error: {httpEx.Message}";
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Critical, $"API Error: {ex.Message}");
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

        public static string MaskApiKey(string? apiKey)
        {
            if (string.IsNullOrEmpty(apiKey)) return "Not provided";
            if (apiKey.Length <= 8) return new string('*', apiKey.Length);
            return apiKey.Substring(0, 3) + new string('*', apiKey.Length - 6) + apiKey.Substring(apiKey.Length - 3);
        }
    }
}
