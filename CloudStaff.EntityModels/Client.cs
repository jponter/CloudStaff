using System;
using System.Collections.Generic;
using System.Text;

namespace CloudStaff.EntityModels
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ClientType Type { get; set; } = ClientType.External;
        public ICollection<ClientProject> Projects { get; set; } = [];

        public string? PrimaryContactName { get; set; }
        public string? PrimaryContactEmail { get; set; }
        public string? AccountManagerName { get; set; } 
        public string? AccountManagerEmail { get; set; }
        public string? ExecutiveSponsorName { get; set; }
        public string? ExecutiveSponsorEmail { get; set; }
        public ClientStatus Status { get; set; } = ClientStatus.Active;

    }
}
