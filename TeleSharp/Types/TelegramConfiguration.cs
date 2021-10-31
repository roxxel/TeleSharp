using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleSharp.Types
{
    public record TelegramConfiguration(
        int ApiId,
        string ApiHash,
        string PhoneNumber,
        string BotToken = null,
        bool UseTestDc = false,
        string DatabaseDirectory = null,
        string FilesDirectory = null,
        bool UseFileDatabase = true,
        bool UseChatInfoDatabase = true,
        bool UseMessageDatabase = true,
        bool UseSecretChats = true,
        string SystemLanguageCode = "En",
        string DeviceModel = "TeleSharp",
        string SystemVersion = "10",
        string ApplicationVersion = "1.0.0",
        bool EnableStorageOptimizer = true,
        bool IgnoreFileNames = false);
}
