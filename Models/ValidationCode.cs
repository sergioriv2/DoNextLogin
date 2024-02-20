using System.ComponentModel.DataAnnotations;

namespace ServerlessLogin.Models
{
    public class ValidationCode : BaseModel
    {
        [Required]
        public string UserId { get; set; }
        public string Code { get; set; }
        public DateTime? ActivatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        [Required]
        public string CodeTypeId { get; set; }
        public User User { get; set; }
        public CodeType CodeType { get; set; }

        public ValidationCode() : base()
        {


        }

        public override string ToString()
        {
            return $"Id: {Id}, UserId: {UserId},  Code: {Code}, CodeTypeId: {CodeTypeId}, ActivatedAt: {ActivatedAt}, " +
                $"ExpiresAt: {ExpiresAt}";
        }
    }
}
