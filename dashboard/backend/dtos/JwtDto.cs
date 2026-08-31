namespace backend.dtos
{
    public class JwtDto
    {
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpireAt { get; set; }
        
    }
}
