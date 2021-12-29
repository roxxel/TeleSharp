using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleSharp.Types.ReplyMarkup
{
    public class InlineButtonLoginUrl : InlineKeyboardButton
    {

        internal InlineButtonLoginUrl(string text, string url, long id, string forwardText = null) : base(text)
        {
            Url = url;
            Id = id;
            ForwardText = forwardText;
        }
        public string Url { get; }
        public long Id { get; }
        public string ForwardText { get; }
    }
}
