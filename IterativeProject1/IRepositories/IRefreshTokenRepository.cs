using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IterativeProject1.Models;

namespace IterativeProject1.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<int> Create(RefreshToken refreshToken);
        Task<RefreshToken> GetByToken(string token);
        Task Revoke(int refreshTokenId, DateTime revokedAt);
        Task RevokeByToken(string token, DateTime revokedAt);
    }
}
