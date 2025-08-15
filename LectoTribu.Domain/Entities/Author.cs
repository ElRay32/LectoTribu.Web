using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LectoTribu.Domain.Abstractions;

namespace LectoTribu.Domain.Entities;

public class Author : BaseEntity
{
    public string Name { get; private set; }
    public Author(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nombre requerido", nameof(name));
        Name = name.Trim();
    }
}
