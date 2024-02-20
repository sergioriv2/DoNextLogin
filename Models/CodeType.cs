using System.ComponentModel;

namespace ServerlessLogin.Models
{

    public enum CodeTypeEnum
    {
        Email,
        Phone,
    }

    public class CodeType : BaseModel
    {
        public string Name { get; set; }

        public CodeType()
        {
            
        }
    }
}
