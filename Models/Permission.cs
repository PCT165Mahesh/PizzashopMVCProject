using System;
using System.Collections.Generic;

namespace PizzashopMVCProject.Models;

public partial class Permission
{
    public long PermissionId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Rolesandpermission> Rolesandpermissions { get; set; } = new List<Rolesandpermission>();
}
