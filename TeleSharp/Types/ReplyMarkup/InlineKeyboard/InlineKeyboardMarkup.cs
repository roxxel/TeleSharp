using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TdLib;

namespace TeleSharp.Types.ReplyMarkup
{
    public class InlineKeyboardMarkup : ReplyMarkup
    {

        internal InlineKeyboardMarkup(List<List<TdApi.InlineKeyboardButton>> rows)
        {
            markup = new TdApi.ReplyMarkup.ReplyMarkupInlineKeyboard
            {
                Rows = rows.Select(x=>x.ToArray()).ToArray()
            };

            Rows = new();
            foreach (var row in rows)
            {
                Rows.Add(row.Select(x =>
                {
                    if (x.Type is TdApi.InlineKeyboardButtonType.InlineKeyboardButtonTypeCallback callback)
                        return new InlineButtonCallbackData(x.Text, callback.Data);
                    else if (x.Type is TdApi.InlineKeyboardButtonType.InlineKeyboardButtonTypeLoginUrl loginUrl)
                        return new InlineButtonLoginUrl(x.Text, loginUrl.Url, loginUrl.Id, loginUrl.ForwardText);
                    else if (x.Type is TdApi.InlineKeyboardButtonType.InlineKeyboardButtonTypeUrl url)
                        return new InlineButtonUrl(x.Text, url.Url);
                    return new InlineKeyboardButton(x.Text);
                }).ToList());
            }
        }

        public List<List<InlineKeyboardButton>> Rows { get; private set; }

    }
}
