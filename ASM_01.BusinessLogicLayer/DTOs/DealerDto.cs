using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_01.BusinessLayer.DTOs
{
    public class DealerDto
    {
        public int DealerId { get; set; }
        public string? Name { get; set; } = null!;
        public string? Address { get; set; }
    }
}
