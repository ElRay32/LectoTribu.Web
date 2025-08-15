using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LectoTribu.Domain.Abstractions
{  
    public abstract class Post : BaseEntity
{
    public Guid UserId { get; protected set; }
    public string Content { get; protected set; }

    protected Post(Guid userId, string content)
    {
        if (string.IsNullOrWhiteSpace(content)) throw new ArgumentException("Contenido requerido", nameof(content));
        UserId = userId;
        Content = content.Trim();
    }

    public abstract string Kind { get; }
} }
