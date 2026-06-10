using System;
using System.Collections.Generic;
using System.Text;

namespace CloudStaff.EntityModels
{
    public class Staff
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ManagerId { get; set; }
        public Manager Manager { get; set; } = null!;
    }
}
