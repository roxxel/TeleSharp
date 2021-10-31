using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TdLib;
using TeleSharp.Enums;

namespace TeleSharp.Types
{
    public class AuthorizationStateChangedEventArgs
    {
        private readonly TdClient _client;
        internal AuthorizationStateChangedEventArgs(TdClient client, AuthorizationState state)
        {
            _client = client;
            State = state;
        }
        public AuthorizationState State { get; }

        public async Task Respond(string value)
        {
            if (State == AuthorizationState.WaitingForLoginCode)
                await _client.CheckAuthenticationCodeAsync(value);
            else if (State == AuthorizationState.WaitingForPassword)
                await _client.CheckAuthenticationPasswordAsync(value);
        }

    }   
}
