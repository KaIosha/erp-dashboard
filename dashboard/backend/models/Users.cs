using System.ComponentModel.DataAnnotations.Schema;


namespace backend.models
{
    public class Users
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

       
        public int RoleId { get; set; }
        public Roles Role { get; set; } = null!;
        public ICollection<RefreshToken> Tokens { get; set; } = new List<RefreshToken>();
    }
}
