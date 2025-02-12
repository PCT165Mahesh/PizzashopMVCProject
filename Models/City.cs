using System;
using System.Collections.Generic;

namespace PizzashopMVCProject.Models;

public partial class City
{
    public long CityId { get; set; }

    public string Name { get; set; } = null!;

    public long Stateid { get; set; }

    public virtual State State { get; set; } = null!;
}
