using System;
using System.Collections.Generic;

namespace ehs5.Models;

public partial class Seller
{
    public int SellerId { get; set; }

    public string UserName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public string PhoneNo { get; set; } = null!;

    public string? Address { get; set; } = null!;

    public int StateId { get; set; }

    public int CityId { get; set; }

    public string EmailId { get; set; } = null!;

    public int? SellerCityId { get; set; }

    public int? SellerStateId { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();

    public virtual City? SellerCity { get; set; }

    public virtual State? SellerState { get; set; }

    public virtual State State { get; set; } = null!;

    public virtual User UserNameNavigation { get; set; } = null!;
}
