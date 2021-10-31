using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeleSharp.Types.ReplyMarkup;


namespace TeleSharp.Types.Builders
{
    public class InlineKeyboardMarkupBuilder
    {
        private List<List<TdLib.TdApi.InlineKeyboardButton>> _rows;


        public InlineKeyboardMarkupBuilder()
        {
            _rows = new();
        }

        /// <summary>
        /// Adds button with callback data to keyboard
        /// </summary>
        /// <param name="text">Text of the button</param>
        /// <param name="callbackData">Data to be sent in a callback query to the bot when button is pressed (1-64 bytes)</param>
        /// <param name="buttonRow">Row where button will be placed (starting from zero)</param>
        /// <returns>Instance of builder</returns>
        public InlineKeyboardMarkupBuilder AddCallbackButton([NotNull] string text, [NotNull] string callbackData, int buttonRow = 0)
        {
            var row = GetRow(buttonRow);

            var data = Encoding.Unicode.GetBytes(callbackData);
            if (data.Length > 64 || data.Length < 1)
                throw new ArgumentOutOfRangeException("Callback data size must be in range (1-64 bytes)");

            row.Add(new TdLib.TdApi.InlineKeyboardButton
            {
                Text = text,
                Type = new TdLib.TdApi.InlineKeyboardButtonType.InlineKeyboardButtonTypeCallback()
                {
                    Data = data
                }
            });
            return this;
        }

        /// <summary>
        /// Adds button with url to keyboard
        /// </summary>
        /// <param name="text">Text of the button</param>
        /// <param name="url">http url to be opened on button click</param>
        /// <param name="buttonRow">Row where button will be placed (starting from zero)</param>
        /// <returns>Instance of builder</returns>
        public InlineKeyboardMarkupBuilder AddUrlButton([NotNull] string text, [NotNull] string url, int buttonRow = 0)
        {
            var row = GetRow(buttonRow);

            row.Add(new TdLib.TdApi.InlineKeyboardButton
            {
                Text = text,
                Type = new TdLib.TdApi.InlineKeyboardButtonType.InlineKeyboardButtonTypeUrl()
                {
                    Url = url
                }
            });
            return this;
        }

        /// <summary>
        /// Adds button with url to keyboard
        /// </summary>
        /// <param name="text">Text of the button</param>
        /// <param name="url">url to be opened with user authorization data added to the query string when the button is pressed</param>
        /// <param name="buttonRow">Row where button will be placed (starting from zero)</param>
        /// <param name="forwardText">New text of the button in forwarded messages</param>
        /// <param name="id">Unique button identifier</param>
        /// <returns>Instance of builder</returns>
        public InlineKeyboardMarkupBuilder AddLoginUrlButton(string text, string url, string forwardText, int id, int buttonRow = 0)
        {
            var row = GetRow(buttonRow);

            row.Add(new TdLib.TdApi.InlineKeyboardButton
            {
                Text = text,
                Type = new TdLib.TdApi.InlineKeyboardButtonType.InlineKeyboardButtonTypeLoginUrl()
                {
                    Url = url,
                    ForwardText = forwardText,
                    Id = id,
                }
            });
            return this;
        }

        public InlineKeyboardMarkup Build()
        {
            return new InlineKeyboardMarkup(_rows);
        }

        private List<TdLib.TdApi.InlineKeyboardButton> GetRow(int buttonRow)
        {
            if (buttonRow > _rows.Count - 1)
            {
                _rows.Add(new List<TdLib.TdApi.InlineKeyboardButton>());
                buttonRow = _rows.Count - 1;
            }

            var row = _rows[buttonRow];
            return row;
        }


    }
}
