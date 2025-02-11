using System;
using System.Collections.Generic;

namespace ehs5.Models;

public partial class PropertyImage
{
    public int PropertyImageId { get; set; }

    public int PropertyId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public int? PropertyImageId1 { get; set; }

    public virtual ICollection<PropertyImage> InversePropertyImageId1Navigation { get; set; } = new List<PropertyImage>();

    public virtual Property Property { get; set; } = null!;

    public virtual PropertyImage? PropertyImageId1Navigation { get; set; }
}
