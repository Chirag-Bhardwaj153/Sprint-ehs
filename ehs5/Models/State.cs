using System;
using System.Collections.Generic;

namespace ehs5.Models;

public partial class State
{
    public int StateId { get; set; }

    public string? StateName { get; set; }

    public virtual ICollection<Buyer> Buyers { get; set; } = new List<Buyer>();

    public virtual ICollection<City> Cities { get; set; } = new List<City>();

    public virtual ICollection<Seller> SellerSellerStates { get; set; } = new List<Seller>();

    public virtual ICollection<Seller> SellerStates { get; set; } = new List<Seller>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
