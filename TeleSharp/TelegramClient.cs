using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using TdLib;
using TeleSharp.Exceptions;
using TeleSharp.Types;
using TeleSharp.UpdateHandling;
using static TdLib.TdApi;

namespace TeleSharp
{
    public sealed class TelegramClient
    {

        private ManualResetEventSlim _resetEvent = new();
        private bool _authNeeded;
        private Enums.AuthorizationState _authorizationState;
        private User _me;
        private readonly TelegramConfiguration _configuration;

        private bool isBot => _configuration.BotToken != null;

        private readonly UpdateHandler _updateHandler;
        internal readonly TdClient _client;
        public TelegramClient(TelegramConfiguration configuration)
        {
            _configuration = configuration;
            if (string.IsNullOrEmpty(configuration.SessionName))
                throw new ArgumentNullException(nameof(configuration.SessionName));
            _updateHandler = new UpdateHandler(this);

            _client = new TdClient();
            _client.Bindings.SetLogVerbosityLevel(configuration.LogVerbosityLevel);
            Console.Clear();
            _client.UpdateReceived += OnUpdateReceived;
            _ = Task.Run(Authorize);
        }

        public event EventHandler<Types.AuthorizationStateChangedEventArgs> AuthorizationStateChanged;

        public TdClient RawClient => _client;
        public User CurrentUser => _me;

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
                    await _client.CheckAuthenticationBotTokenAsync(_configuration.BotToken);
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
            if (state == Enums.AuthorizationState.Ready)
            {
                _me = await GetMeAsync();
                if (_me.Type.GetType() != typeof(UserType.UserTypeBot))
                    await _client.GetChatsAsync(null, 1000);
            }
            AuthorizationStateChanged?.Invoke(this, new(_client, state));
        }

        private async void OnUpdateReceived(object sender, TdApi.Update update)
        {
            Parallel.ForEach(_updateHandler._handlers.Where(x => x.TdUpdateType == update.GetType()), async (handler) =>
            {
                _ = _updateHandler.Execute(handler, update);
            });

            switch (update)
            {
                //Authorization logic below 
                case TdApi.Update.UpdateAuthorizationState updateAuthorizationState when updateAuthorizationState.AuthorizationState.GetType() == typeof(TdApi.AuthorizationState.AuthorizationStateWaitTdlibParameters):
                    {
                        string dbDir = string.Empty;
                        if (string.IsNullOrEmpty(_configuration.DatabaseDirectory)) dbDir = Path.Combine(Environment.CurrentDirectory, _configuration.SessionName);
                        else dbDir = Path.Combine(_configuration.DatabaseDirectory, _configuration.SessionName);

                        if (!Directory.Exists(dbDir))
                            Directory.CreateDirectory(dbDir);

                        string filesDir = string.Empty;
                        if (string.IsNullOrEmpty(_configuration.DatabaseDirectory)) filesDir = Path.Combine(Environment.CurrentDirectory, _configuration.SessionName, "files");
                        else filesDir = Path.Combine(_configuration.FilesDirectory, _configuration.SessionName);

                        if (!Directory.Exists(filesDir))
                            Directory.CreateDirectory(filesDir);

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
                                DatabaseDirectory = dbDir,
                                FilesDirectory = filesDir,
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
                    }
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
