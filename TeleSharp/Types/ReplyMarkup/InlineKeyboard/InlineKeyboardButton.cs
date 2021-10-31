using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleSharp.Types.ReplyMarkup
{
    public class InlineKeyboardButton
    {
        public string Text { get; }
        internal InlineKeyboardButton(string text)
        {
            Text = text;
        }

    }
}
