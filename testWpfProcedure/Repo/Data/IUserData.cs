using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testWpfProcedure.Repo.Data
{
    public interface IUserData
    {
        Task DeleteUser(int id);
        Task<User> GetUser(int id);
        Task<IEnumerable<User>> GetUsers();
        Task InsertUser(User user);
        Task UpdateUser(User user);
    }
}
