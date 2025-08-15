using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LectoTribu.Domain.ValueObjects
{
    public sealed record Rating
    {
        public int Value { get; }
        public Rating(int value)
        {
            if (value is < 1 or > 5) throw new ArgumentOutOfRangeException(nameof(value), "Rating 1..5");
            Value = value;
        }
        public static implicit operator int(Rating r) => r.Value;
    }
}
