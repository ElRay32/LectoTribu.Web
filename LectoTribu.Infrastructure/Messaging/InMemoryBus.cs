using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using LectoTribu.Domain.Abstractions;

namespace LectoTribu.Infrastructure.Messaging;

public class InMemoryBus : IMessageBus
{
    private readonly ConcurrentDictionary<Type, List<Func<object, Task>>> _handlers = new();

    public Task PublishAsync<T>(T message, CancellationToken ct = default)
    {
        if (_handlers.TryGetValue(typeof(T), out var typed))
            return Task.WhenAll(typed.Select(h => h(message!)));
        if (message != null && _handlers.TryGetValue(message.GetType(), out var dyn))
            return Task.WhenAll(dyn.Select(h => h(message!)));
        return Task.CompletedTask;
    }

    public void Subscribe<T>(Func<T, Task> handler)
    {
        var list = _handlers.GetOrAdd(typeof(T), _ => new());
        list.Add(msg => handler((T)msg));
    }
}