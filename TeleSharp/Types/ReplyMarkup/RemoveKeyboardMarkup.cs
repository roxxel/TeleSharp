using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleSharp.Types.ReplyMarkup
{
    public class RemoveKeyboardMarkup : ReplyMarkup
    {       
        public RemoveKeyboardMarkup(bool isPersonal)
        {
            IsPersonal = isPersonal;
            markup = new TdLib.TdApi.ReplyMarkup.ReplyMarkupRemoveKeyboard { IsPersonal = IsPersonal };
        }

        public bool IsPersonal { get; }
    }
}
