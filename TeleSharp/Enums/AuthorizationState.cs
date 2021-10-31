using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleSharp.Enums
{
    public enum AuthorizationState
    {
        Ready,
        WaitingForLoginCode,
        WaitingForPassword
    }
}
