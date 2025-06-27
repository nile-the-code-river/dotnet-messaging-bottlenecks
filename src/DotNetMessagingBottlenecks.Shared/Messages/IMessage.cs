using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetMessagingBottlenecks.Shared.Messages
{
    internal class IMessage
    {
        Guid MessageId { get; }
        DateTime Timestamp { get; }
    }
}
