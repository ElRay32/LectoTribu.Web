using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LectoTribu.Domain.Abstractions;

namespace LectoTribu.Domain.Entities;

public class Membership : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid ClubId { get; private set; }
    public ClubRole Role { get; private set; }

    public Membership(Guid userId, Guid clubId, ClubRole role)
    {
        UserId = userId;
        ClubId = clubId;
        Role = role;
    }
}
