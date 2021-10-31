using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeleSharp.Enums;

namespace TeleSharp.Types.ReplyMarkup
{
    public class KeyboardButton
    {
        public string Text { get; }
        public KeyboardButtonAction Action { get; }

        internal KeyboardButton(string text, KeyboardButtonAction action)
        {
            Text = text;
            Action = action;
        }
    }
}
