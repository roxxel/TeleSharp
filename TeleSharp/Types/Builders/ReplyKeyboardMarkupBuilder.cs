using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeleSharp.Enums;
using TeleSharp.Types.ReplyMarkup;
using static TdLib.TdApi;

namespace TeleSharp.Types.Builders
{
    public class ReplyKeyboardMarkupBuilder
    {
        private List<List<TdLib.TdApi.KeyboardButton>> _rows;

        public ReplyKeyboardMarkupBuilder(bool isPersonal = false, bool resizeKeyboard = false, bool oneTime = false)
        {
            _rows = new();

            IsPersonal = isPersonal;
            ResizeKeyboard = resizeKeyboard;
            OneTime = oneTime;
        }

        public bool IsPersonal { get; }
        public bool ResizeKeyboard { get; }
        public bool OneTime { get; }

        public ReplyKeyboardMarkupBuilder AddButton(string text, int buttonRow = 0, KeyboardButtonAction action = KeyboardButtonAction.None)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentException($"'{nameof(text)}' cannot be null or empty.", nameof(text));
            var row = GetRow(buttonRow);

            KeyboardButtonType type = null;
            if (action == KeyboardButtonAction.RequestLocation)
                type = new KeyboardButtonType.KeyboardButtonTypeRequestLocation(); 
            else if (action == KeyboardButtonAction.RequestContact)
                type = new KeyboardButtonType.KeyboardButtonTypeRequestPhoneNumber();
            else
                type = new KeyboardButtonType.KeyboardButtonTypeText();

            row.Add(new TdLib.TdApi.KeyboardButton
            {
                Text = text,
                Type = type
            });

            return this;
        }
        public ReplyKeyboardMarkup Build()
        {
            return new ReplyKeyboardMarkup(_rows, IsPersonal, ResizeKeyboard, OneTime);
        }

        private List<TdLib.TdApi.KeyboardButton> GetRow(int buttonRow)
        {
            if (buttonRow > _rows.Count - 1)
            {
                _rows.Add(new List<TdLib.TdApi.KeyboardButton>());
                buttonRow = _rows.Count - 1;
            }

            var row = _rows[buttonRow];
            return row;
        }


    }
}
