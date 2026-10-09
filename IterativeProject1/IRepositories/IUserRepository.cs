using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IterativeProject1.Models;
using IterativeProject1.DTOs;

namespace IterativeProject1.Repositories
{
    public interface IUserRepository
    {
       Task<User>GetByEmailOrUserName(string identifier);
        Task<int> Create(User user);
        Task<bool> UsernameExists(string username);
        Task<bool> EmailExists(string email);
        Task<User> GetById(int userId);
        Task<IEnumerable<User>> GetByRole(string role);
    };
}
