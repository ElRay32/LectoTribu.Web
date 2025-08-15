using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LectoTribu.Domain.Abstractions
{
    public interface IMessageBus
    {
        Task PublishAsync<T>(T message, CancellationToken ct = default);
        void Subscribe<T>(Func<T, Task> handler);
    }
}