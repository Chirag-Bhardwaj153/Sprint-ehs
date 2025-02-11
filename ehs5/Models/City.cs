using System;
using System.Collections.Generic;

namespace ehs5.Models;

public partial class City
{
    public int CityId { get; set; }

    public string CityName { get; set; } = null!;

    public int? StateId { get; set; }

    public virtual ICollection<Buyer> Buyers { get; set; } = new List<Buyer>();

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();

    public virtual ICollection<Seller> SellerCities { get; set; } = new List<Seller>();

    public virtual ICollection<Seller> SellerSellerCities { get; set; } = new List<Seller>();

    public virtual State? State { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
