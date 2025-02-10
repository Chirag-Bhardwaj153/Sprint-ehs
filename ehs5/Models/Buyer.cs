using System;
using System.Collections.Generic;

namespace ehs5.Models;

public partial class Buyer
{
    public int BuyerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public string PhoneNo { get; set; } = null!;

    public string EmailId { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public int? BuyerStateId { get; set; }

    public int? BuyerCityId { get; set; }

    public virtual City? BuyerCity { get; set; }

    public virtual State? BuyerState { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual User UserNameNavigation { get; set; } = null!;
}
