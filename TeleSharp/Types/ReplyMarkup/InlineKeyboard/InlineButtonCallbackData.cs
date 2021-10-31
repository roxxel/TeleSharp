using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleSharp.Types.ReplyMarkup
{
    public class InlineButtonCallbackData : InlineKeyboardButton
    {
        private byte[] _data;

        internal InlineButtonCallbackData(string text, byte[] data) : base(text)
        {
            _data = data;
        }

        public string CallbackData => Encoding.Unicode.GetString(_data);

    }
}
