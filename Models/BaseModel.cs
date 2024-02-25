using System.ComponentModel.DataAnnotations;

namespace ServerlessLogin.Models
{
    public abstract class BaseModel
    {
        [Key]
        [Required]
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
        public DateTime? DeletedAt { get; set; } = null;

        public BaseModel()
        {
            Id = Guid.NewGuid().ToString();
        }

    }
}
