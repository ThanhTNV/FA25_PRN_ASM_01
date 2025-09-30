using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASM_01.BusinessLayer.DTOs
{
    public class AuthDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;

        public string Role { get; set; } = null!;
    }
}
