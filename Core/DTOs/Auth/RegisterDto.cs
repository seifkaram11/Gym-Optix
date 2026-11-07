using Core.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs
{
    public class RegisterDto
    {
        public string? UserName { get; set; } 
        public string? Email { get; set; } 
        public string? Password { get; set; } 
        public Gender? Gender { get; set; }
    }
}
