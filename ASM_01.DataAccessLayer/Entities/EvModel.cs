using ASM_01.DataAccessLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_01.DataAccessLayer.Entities;

public class EvModel
{
    public int EvModelId { get; set; }
    public string ModelName { get; set; } = null!;
    public string? Description { get; set; }
    public EvStatus Status { get; set; } = EvStatus.Unavailable;

    public virtual ICollection<EvTrim> Trims { get; set; } = new List<EvTrim>();
}
