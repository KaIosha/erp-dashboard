namespace backend.dtos
{
    public class AuthResponseDto
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Message { get; set; }
    }
}
