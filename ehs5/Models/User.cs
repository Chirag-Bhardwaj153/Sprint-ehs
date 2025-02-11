using System;
using System.Collections.Generic;

namespace ehs5.Models;

public partial class User
{
    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? UserType { get; set; }

    public int? UserCityId { get; set; }

    public int? UserStateId { get; set; }

    public virtual ICollection<Buyer> Buyers { get; set; } = new List<Buyer>();

    public virtual ICollection<Seller> Sellers { get; set; } = new List<Seller>();

    public virtual City? UserCity { get; set; }

    public virtual State? UserState { get; set; }
}
