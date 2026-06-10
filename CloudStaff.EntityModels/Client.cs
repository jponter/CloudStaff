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
        public ICollection<ClientProject> Projects { get; set; } = new List<ClientProject>();



    }

    public enum ClientType
    {
        Internal,
        External,

    }
}
