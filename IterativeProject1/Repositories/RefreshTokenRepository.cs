using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IterativeProject1.Data;
using IterativeProject1.Models;
using Dapper;

namespace IterativeProject1.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly DapperContext _context;

        public RefreshTokenRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> Create(RefreshToken refreshToken)
        {
            var sql = @"INSERT INTO RefreshToken (UserId, Token, ExpiresAt) VALUES (@UserId, @Token,@ExpiresAt); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(
                sql,
                refreshToken);
        }

        public async Task<RefreshToken> GetByToken(string token)
        {
            var sql = @"SELECT * FROM RefreshToken WHERE Token = @Token";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleOrDefaultAsync<RefreshToken>(
                sql,
                new { Token = token });
        }

        public async Task Revoke(int refreshTokenId, DateTime revokedAt)
        {
            var sql = @"UPDATE RefreshToken SET RevokedAt = @RevokedAt WHERE RefreshTokenId = @RefreshTokenId";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                sql,
                new { RevokedAt = revokedAt, RefreshTokenId = refreshTokenId });
        }

        public async Task RevokeByToken(string token, DateTime revokedAt)
        {
            var sql = @"UPDATE RefreshToken SET RevokedAt = @RevokedAt WHERE Token = @Token AND RevokedAt IS NULL";
            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                sql,
                new { RevokedAt = revokedAt, Token = token });
        }
    }
}
