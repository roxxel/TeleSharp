using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static TdLib.TdApi;

namespace TeleSharp.UpdateHandling
{
    internal class UpdateHandler
    {
        private readonly TelegramClient _tgClient;
        private readonly Dictionary<UpdateHandlerType, Type> _signatures;
        private TdLib.TdClient _client => _tgClient._client;

        internal List<UpdateHandlerMethod> _handlers;

        public UpdateHandler(TelegramClient client)
        {
            _tgClient = client;
            _signatures = new Dictionary<UpdateHandlerType, Type>()
            {
                [UpdateHandlerType.Message] = typeof(Update.UpdateNewMessage),
                [UpdateHandlerType.UserStatusUpdated] = typeof(Update.UpdateUserStatus)
            };
            _handlers = new();
            Initialize();
        }


        internal async Task Execute(UpdateHandlerMethod handler, Update value)
        {
            try
            {
                var task = (Task)handler.Method.Invoke(handler.ContainingType, new object[] { _tgClient, value });
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            await Task.CompletedTask;
        }

        private void Initialize()
        {
            var asm = Assembly.GetEntryAssembly();
            var types = asm.GetTypes().Where(x => x.IsPublic && x.BaseType == typeof(BaseUpdateHandlerModule)).ToArray();
            foreach (var type in types)
            {
                if (!type.GetConstructors().Any(x => x.IsPublic && x.GetParameters().Length == 0))
                    throw new Exception($"{type.FullName} must have at least one public parameterless constructor");
                var methods = type.GetMethods().Where(x => x.GetCustomAttribute<UpdateHandlerAttribute>() != null && x.IsPublic);
                foreach (var method in methods)
                {
                    var attr = method.GetCustomAttribute<UpdateHandlerAttribute>();
                    var @params = method.GetParameters();
                    var signature = _signatures[attr.Type];
                    if (method.ReturnType != typeof(Task))
                        throw new Exception($"{method.Name} must have return type of System.Threading.Tasks.Task");
                    if (@params.Length != 2)
                        throw new Exception($"{method.Name} expected parameters count 2");
                    if (@params[0].ParameterType != typeof(TelegramClient) || @params[1].ParameterType != signature)
                        throw new Exception($"{method.Name} have invalid signature. Expected (TelegramClient, {signature.Name})");
                    _handlers.Add(new()
                    {
                        ContainingType = Activator.CreateInstance(type),
                        Method = method,
                        TdUpdateType = GetRelatedTdUpdateType(attr.Type),
                        Type = attr.Type
                    });
                }
            }
        }

        private Type GetRelatedTdUpdateType(UpdateHandlerType type)
        {
            return type switch
            {
                UpdateHandlerType.Message => typeof(Update.UpdateNewMessage),
                UpdateHandlerType.UserStatusUpdated => typeof(Update.UpdateUserStatus),

                _ => throw new NotImplementedException(),
            };
        }
    }
}
