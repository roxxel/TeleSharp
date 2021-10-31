using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TdLib;

namespace TeleSharp.Types.ReplyMarkup
{
    public class ReplyKeyboardMarkup : ReplyMarkup
    {
        internal ReplyKeyboardMarkup(List<List<TdApi.KeyboardButton>> rows, bool isPersonal, bool resizeKeyboard, bool oneTime)
        {
            markup = new TdApi.ReplyMarkup.ReplyMarkupShowKeyboard()
            {
                Rows = rows.Select(x => x.ToArray()).ToArray(),
                OneTime = oneTime,
                IsPersonal = isPersonal,
                ResizeKeyboard = resizeKeyboard
            };

            IsPersonal = isPersonal;
            ResizeKeyboard = resizeKeyboard;
            OneTime = oneTime;

            var _rows = new List<IEnumerable<KeyboardButton>>();
            foreach (var row in rows)
            {
                _rows.Add(row.Select(x =>
                {
                    if (x.Type is TdApi.KeyboardButtonType.KeyboardButtonTypeRequestLocation loc)
                        return new KeyboardButton(x.Text, Enums.KeyboardButtonAction.RequestLocation);
                    else if (x.Type is TdApi.KeyboardButtonType.KeyboardButtonTypeRequestPhoneNumber phone)
                        return new KeyboardButton(x.Text, Enums.KeyboardButtonAction.RequestContact);
                    return new KeyboardButton(x.Text, Enums.KeyboardButtonAction.None);
                }));
            }
            Rows = _rows.ToArray();
        }

        public IEnumerable<IEnumerable<KeyboardButton>> Rows { get; }

        public bool IsPersonal { get; }
        public bool ResizeKeyboard { get; }
        public bool OneTime { get; }
    }
}
