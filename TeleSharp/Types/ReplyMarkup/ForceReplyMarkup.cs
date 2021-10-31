using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleSharp.Types.ReplyMarkup
{
    public class ForceReplyMarkup : ReplyMarkup
    {
        public ForceReplyMarkup(bool isPersonal = false)
        {
            IsPersonal = isPersonal;
            markup = new TdLib.TdApi.ReplyMarkup.ReplyMarkupForceReply { IsPersonal = IsPersonal };
        }

        public bool IsPersonal { get; }
    }
}
