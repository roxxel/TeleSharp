using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using TdLib;
using TeleSharp.Exceptions;
using TeleSharp.Types;
using static TdLib.TdApi;

namespace TeleSharp
{
    public sealed class TelegramClient
    {

        private ManualResetEventSlim _resetEvent = new();
        private bool _authNeeded;
        private Enums.AuthorizationState _authorizationState;
        private readonly TelegramConfiguration _configuration;

        private bool isBot => _configuration.BotToken != null;

        internal readonly TdClient _client;
        public TelegramClient(TelegramConfiguration configuration)
        {
            _configuration = configuration;

            _client = new TdClient();
            _client.Bindings.SetLogVerbosityLevel(0);
            Console.Clear();
            _client.UpdateReceived += OnUpdateReceived;
            _ = Task.Run(Authorize);
        }

        public event EventHandler<Types.AuthorizationStateChangedEventArgs> AuthorizationStateChanged;


        public async Task<User> GetMeAsync()
        {
            EnsureClientReady();

            return await _client.GetMeAsync();
        }

        private async Task Authorize()
        {
            _resetEvent.Wait();
            if (_authNeeded)
            {
                if (_configuration.BotToken != null)
                {
                    try
                    {
                        await _client.CheckAuthenticationBotTokenAsync(_configuration.BotToken);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    return;
                }
                await _client.ExecuteAsync(new TdApi.SetAuthenticationPhoneNumber
                {
                    PhoneNumber = _configuration.PhoneNumber
                });
                OnStateChanged(Enums.AuthorizationState.WaitingForLoginCode);
            }
        }

        internal void EnsureClientReady()
        {
            if (_authorizationState != Enums.AuthorizationState.Ready)
                throw new UnauthorizedException("Unauthorized");
        }

        private async void OnStateChanged(Enums.AuthorizationState state)
        {
            _authorizationState = state;
            AuthorizationStateChanged?.Invoke(this, new(_client, state));
        }

        private async void OnUpdateReceived(object sender, TdApi.Update update)
        {
            switch (update)
            {
                //Authorization logic below 
                case TdApi.Update.UpdateAuthorizationState updateAuthorizationState when updateAuthorizationState.AuthorizationState.GetType() == typeof(TdApi.AuthorizationState.AuthorizationStateWaitTdlibParameters):
                    await _client.ExecuteAsync(new TdApi.SetTdlibParameters
                    {
                        Parameters = new TdApi.TdlibParameters
                        {
                            ApiId = _configuration.ApiId,
                            ApiHash = _configuration.ApiHash,
                            ApplicationVersion = _configuration.ApplicationVersion,
                            DeviceModel = _configuration.DeviceModel,
                            SystemLanguageCode = _configuration.SystemLanguageCode,
                            SystemVersion = _configuration.SystemVersion,
                            DatabaseDirectory = _configuration.DatabaseDirectory,
                            FilesDirectory = _configuration.FilesDirectory,
                            IgnoreFileNames = _configuration.IgnoreFileNames,
                            EnableStorageOptimizer = _configuration.EnableStorageOptimizer,
                            UseChatInfoDatabase = _configuration.UseChatInfoDatabase,
                            UseFileDatabase = _configuration.UseFileDatabase,
                            UseMessageDatabase = _configuration.UseMessageDatabase,
                            UseSecretChats = _configuration.UseSecretChats,
                            UseTestDc = _configuration.UseTestDc
                        }
                    });
                    break;
                case TdApi.Update.UpdateAuthorizationState updateAuthorizationState when updateAuthorizationState.AuthorizationState.GetType() == typeof(TdApi.AuthorizationState.AuthorizationStateWaitEncryptionKey):
                    await _client.ExecuteAsync(new TdApi.CheckDatabaseEncryptionKey());
                    break;
                case TdApi.Update.UpdateAuthorizationState updateAuthorizationState when updateAuthorizationState.AuthorizationState.GetType() == typeof(TdApi.AuthorizationState.AuthorizationStateWaitPhoneNumber):
                    _authNeeded = true;
                    _resetEvent.Set();
                    break;

                case TdApi.Update.UpdateAuthorizationState updateAuthorizationState when updateAuthorizationState.AuthorizationState.GetType() == typeof(TdApi.AuthorizationState.AuthorizationStateWaitPassword):
                    {
                        OnStateChanged(Enums.AuthorizationState.WaitingForPassword);
                        break;
                    }
                case TdApi.Update.UpdateAuthorizationState updateAuthorizationState when updateAuthorizationState.AuthorizationState.GetType() == typeof(TdApi.AuthorizationState.AuthorizationStateWaitCode):
                    _authNeeded = true;
                    _resetEvent.Set();
                    break;
                case TdApi.Update.UpdateUser updateUser:
                    _resetEvent.Set();
                    break;

                case TdApi.Update.UpdateAuthorizationState updateAuthorizationState when updateAuthorizationState.AuthorizationState.GetType() == typeof(TdApi.AuthorizationState.AuthorizationStateReady):
                    OnStateChanged(Enums.AuthorizationState.Ready);
                    break;

            }
        }
    }

}
