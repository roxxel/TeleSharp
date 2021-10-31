using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleSharp.Enums
{
    public enum KeyboardButtonAction
    {
        None,
        /// <summary>
        /// User’s location will be sent when the button is pressed. Available in private chats only.
        /// </summary>
        RequestLocation,
        /// <summary>
        /// User’s phone number will be sent as a contact when the button is pressed. Available in private chats only.
        /// </summary>
        RequestContact
    }
}
