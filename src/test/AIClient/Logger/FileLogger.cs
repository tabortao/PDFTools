using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AIClient.Logger
{
    public class FileLogger
    {
        private readonly string _logFilePath;
        private readonly LogLevel _minLogLevel;
        private readonly object _lock = new object();

        public FileLogger(string logFilePath, LogLevel minLogLevel = LogLevel.Information)
        {
            _logFilePath = logFilePath;
            _minLogLevel = minLogLevel;
            
            // 确保日志目录存在
            var logDirectory = Path.GetDirectoryName(_logFilePath);
            if (!string.IsNullOrEmpty(logDirectory) && !Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            
            // 创建日志文件头
            if (!File.Exists(_logFilePath))
            {
                File.WriteAllText(_logFilePath, $"AI Client Log - {DateTime.Now}\n\n");
            }
        }

        public void Log(LogLevel level, string message)
        {
            if (level < _minLogLevel) return;
            
            var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}\n";
            
            lock (_lock)
            {
                File.AppendAllText(_logFilePath, logEntry, Encoding.UTF8);
            }
        }

        public async Task LogAsync(LogLevel level, string message)
        {
            if (level < _minLogLevel) return;
            
            var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}\n";
            
            await Task.Run(() => 
            {
                lock (_lock)
                {
                    File.AppendAllText(_logFilePath, logEntry, Encoding.UTF8);
                }
            });
        }
    }
}