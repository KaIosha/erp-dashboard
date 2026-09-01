using backend.models;

namespace backend.dtos
{
    public class GetRoleDataDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
    }
}
