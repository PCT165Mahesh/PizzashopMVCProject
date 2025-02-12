using System;
using System.Collections.Generic;

namespace PizzashopMVCProject.Models;

public partial class TableStatus
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();
}
