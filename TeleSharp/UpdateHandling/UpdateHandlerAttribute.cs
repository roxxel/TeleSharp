using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleSharp.UpdateHandling
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UpdateHandlerAttribute : Attribute
    {
        public UpdateHandlerAttribute(UpdateHandlerType type)
        {
            Type = type;
        }

        public UpdateHandlerType Type { get; }
    }

    public enum UpdateHandlerType
    {
        Message
    }
}
