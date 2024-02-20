using Microsoft.EntityFrameworkCore;
using ServerlessLogin.Data;
using ServerlessLogin.Models;

namespace ServerlessLogin.Seeders
{
    public class Seed
    {
        private readonly DataContext _dataContext;

        public Seed(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void SeedDataContext()
        {
            var anyCodeTypes = _dataContext.CodeTypes.Any();

            if (!anyCodeTypes)
            {
                var CodeTypes = new List<CodeType>()
                {
                    new()
                    {
                        Name = "Email",
                        Id = ((int)CodeTypeEnum.Email).ToString()
                    },
                    new()
                    {
                        Name = "Phone",
                        Id = ((int) CodeTypeEnum.Phone).ToString()
                    }
                };

                _dataContext.CodeTypes.AddRange(CodeTypes);
                _dataContext.SaveChanges();
            }

            return;
        }
    }
}
