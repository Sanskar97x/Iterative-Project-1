using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using IterativeProject1.Models;
using IterativeProject1.DTOs;
using IterativeProject1.Data;

namespace IterativeProject1.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<User> GetByEmailOrUserName(string identifier)
        {
            var query = "SELECT * FROM [User] WHERE Email = @Identifier OR Username = @Identifier";
            using var connection = _context.CreateConnection();
            var user = await connection.QueryFirstOrDefaultAsync<User>(query, new { Identifier = identifier });
            return user;
        }

        public async Task<int> Create(User user)
        {
            var query = @"INSERT INTO [User] 
                (Username, Email, Password, FullName, Profession, PreferredCalendar, Role, AuthProvider, IsEmailVerified, EmailVerificationToken, EmailVerificationExpiresAt)
                VALUES 
                (@Username, @Email, @Password, @FullName, @Profession, @PreferredCalendar, @Role, @AuthProvider, @IsEmailVerified, @EmailVerificationToken, @EmailVerificationExpiresAt);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = _context.CreateConnection();
            var newUserId = await connection.ExecuteScalarAsync<int>(query, user);
            return newUserId;
        }

        public async Task<bool> UsernameExists(string username)
        {
            var query = "SELECT COUNT(1) FROM [User] WHERE Username = @Username";
            using var connection = _context.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(query, new { Username = username });
            return count > 0;
        }

        public async Task<bool> EmailExists(string email)
        {
            var query = "SELECT COUNT(1) FROM [User] WHERE Email = @Email";
            using var connection = _context.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(query, new { Email = email });
            return count > 0;
        }

        public async Task<User> GetById(int userId)
        {
            var query = "SELECT * FROM [User] WHERE UserId = @UserId";
            using var connection = _context.CreateConnection();
            var user = await connection.QueryFirstOrDefaultAsync<User>(query, new { UserId = userId });
            return user;
        }

        public async Task<IEnumerable<User>> GetByRole(string role)
        {
            var query = "SELECT * FROM [User] WHERE Role = @Role";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<User>(query, new { Role = role });
        }
    }
}