using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TdLib;
using TeleSharp.Enums;
using static TdLib.TdApi;

namespace TeleSharp
{
    public static partial class ClientExtensions
    {
        /// <summary>
        /// Send text message
        /// </summary>
        /// <param name="chatId">Chat id</param>
        /// <param name="message">Message text</param>
        /// <param name="parseMode">Parse mode</param>
        /// <param name="disableNotification">Indicates whether the message will be sent without notification</param>
        /// <param name="replyToMessageId">if message is reply, id of the original message</param>
        /// <param name="markup">Reply markup</param>
        /// <param name="scheduleDate">Unix time when the message will be automatically sent. (any negative value to send when user comes online)</param>
        /// <returns><see cref="Message"/></returns>
        public static async Task<Message> SendMessageAsync(this TelegramClient tgClient,
            long chatId,
            string message,
            ParseMode parseMode = ParseMode.Markdown,
            bool disableNotification = false,
            long replyToMessageId = 0,
            long scheduleDate = 0,
            Types.ReplyMarkup.ReplyMarkup markup = null)
        {
            tgClient.EnsureClientReady();
            var client = tgClient._client;
            await client.GetChatAsync(chatId);

            var text = parseMode == ParseMode.None ?
                new FormattedText
                {
                    Text = message,
                } : await client.ParseTextEntitiesAsync(message,
                parseMode == ParseMode.Markdown ? new TextParseMode.TextParseModeMarkdown() : new TextParseMode.TextParseModeHTML());

            var sendOptions = new MessageSendOptions
            {
                DisableNotification = disableNotification,
            };
            if (scheduleDate != 0)
                sendOptions.SchedulingState = scheduleDate < 0 
                    ? new MessageSchedulingState.MessageSchedulingStateSendWhenOnline()
                    : new MessageSchedulingState.MessageSchedulingStateSendAtDate { SendDate = (int)scheduleDate };

            return await client.SendMessageAsync(chatId, 0, replyToMessageId, sendOptions, markup?.markup, new InputMessageContent.InputMessageText
            {
                Text = text
            });
        }
    }

}
