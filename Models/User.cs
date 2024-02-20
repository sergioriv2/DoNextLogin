namespace ServerlessLogin.Models
{
    public class User : BaseModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<ValidationCode> ValidationCodes { get; set; }
        public List<RefreshToken> RefrehsTokens { get; set; }

        public User() : base()
        {
            
        }
    }
}
