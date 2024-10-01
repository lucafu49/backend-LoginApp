namespace backend_LoginApp.Models
{
    public class PasswordResetRequest
    {
        public string Mail { get;set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
