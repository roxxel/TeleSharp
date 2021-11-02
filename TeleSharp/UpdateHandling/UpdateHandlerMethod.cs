using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace TeleSharp.UpdateHandling
{
    internal class UpdateHandlerMethod
    {
        public MethodInfo Method { get; set; }
        public UpdateHandlerType Type { get; set; }
        public Type TdUpdateType { get; set; }
        public object ContainingType { get; set; }
    }
}
