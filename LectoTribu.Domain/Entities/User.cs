using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LectoTribu.Domain.Abstractions;
using LectoTribu.Domain.ValueObjects;

namespace LectoTribu.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public string? Bio { get; private set; }

    public User(string name, Email email, string? bio = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nombre requerido", nameof(name));
        Name = name.Trim();
        Email = email;
        Bio = bio;
    }
}
