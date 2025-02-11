using System;
using System.Collections.Generic;

namespace ehs5.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public int BuyerId { get; set; }

    public int PropertyId { get; set; }

    public virtual Buyer Buyer { get; set; } = null!;

    public virtual Property Property { get; set; } = null!;
}
