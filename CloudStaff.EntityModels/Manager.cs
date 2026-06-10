using System;
using System.Collections.Generic;
using System.Text;

namespace CloudStaff.EntityModels
{
    public class Manager
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string AsNumber { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public ICollection<Staff> Staff { get; set; } = [];

    }
}
