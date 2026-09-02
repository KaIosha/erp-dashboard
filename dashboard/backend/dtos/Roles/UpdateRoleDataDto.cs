namespace backend.dtos
{
    public class UpdateRoleDataDto
    {
        public string? Name { get; set; }
        public List<string>? Permissions { get; set; }
    }
}
