using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TdLib;
using static TdLib.TdApi;

namespace TeleSharp
{
    public static partial class ClientExtensions
    {
        public static async Task<IEnumerable<TdApi.Chat>> GetChatsAsync(this TelegramClient tgClient)
        {
            tgClient.EnsureClientReady();
            var _client = tgClient._client;
            var chats = (await _client.GetChatsAsync(null, 1000))
                .ChatIds.Select(async x => await _client.GetChatAsync(x))
                .Select(t => t.Result);
            return chats;
        }
    }
}
