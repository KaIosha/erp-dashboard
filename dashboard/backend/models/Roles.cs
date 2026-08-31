namespace backend.models
{
    public class Roles
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
        public List<Users> Users { get; set; } = new();
    }
}
