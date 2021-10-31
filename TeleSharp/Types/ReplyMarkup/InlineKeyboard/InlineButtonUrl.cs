using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleSharp.Types.ReplyMarkup
{
    public class InlineButtonUrl : InlineKeyboardButton
    {
        internal InlineButtonUrl(string text, string url) : base(text)
        {
            Url = url;
        }
        public string Url { get; }
    }
}
