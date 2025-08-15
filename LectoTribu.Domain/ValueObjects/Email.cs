using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LectoTribu.Domain.ValueObjects
{
    public sealed record Email
    {
        public string Value { get; }
        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !value.Contains('@'))
                throw new ArgumentException("Email inválido", nameof(value));
            Value = value.Trim();
        }
        public override string ToString() => Value;
    }
}
