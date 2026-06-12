namespace CloudStaff.EntityModels
{
    public class ClientProject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Client Client { get; set; } = null!;

        public int ClientId { get; set; }
        public string? ProjectManagerName { get; set; }
        public string? ProjectManagerEmail { get; set; }
        public DateOnly? StartDate { get; set; }
        public  DateOnly? EndDate { get; set; }
        public ClientProjectStatus Status { get; set; } = ClientProjectStatus.Active;
    }
}