namespace FamilyApi.DataModels
{
    public class UserRequest
    {
        public required string Name { get; set; }
        public required string Password { get; set; }
        public string? Photo { get; set; }
    }
}
