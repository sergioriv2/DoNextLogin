namespace ServerlessLogin.Models
{
    public class RefreshToken : BaseModel
    {
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public User User { get; set; }

        public RefreshToken() : base()
        {
            
        }
    }
}
