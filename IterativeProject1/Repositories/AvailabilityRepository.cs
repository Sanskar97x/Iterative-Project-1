using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IterativeProject1.Models;
using Dapper;
using IterativeProject1.Data;

namespace IterativeProject1.Repositories
{
    public class AvailabilityRepository : IAvailabilityRepository
    {
        private readonly DapperContext _context;

        public AvailabilityRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> Create(Availability availability)
        {
            var sql = @"INSERT INTO Availability (ProviderId, DayOfWeek, SpecificDate, StartTime, EndTime, IsActive, CreatedAt)
                        VALUES (@ProviderId, @DayOfWeek, @SpecificDate, @StartTime, @EndTime, @IsActive, GETUTCDATE());
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql, availability);
        }

        public async Task<IEnumerable<Availability>> GetByProvider(int providerId)
        {
            var sql = @"SELECT * FROM Availability 
                        WHERE ProviderId = @ProviderId AND IsActive = 1
                        ORDER BY SpecificDate, DayOfWeek, StartTime";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Availability>(sql, new { ProviderId = providerId });
        }

        public async Task<Availability> GetById(int id)
        {
            var sql = @"SELECT * FROM Availability WHERE Id = @Id";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Availability>(sql, new { Id = id });
        }

        public async Task<bool> Delete(int id)
        {
            var sql = @"UPDATE Availability SET IsActive = 0 WHERE Id = @Id";
            using var connection = _context.CreateConnection();
            var rows = await connection.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }
        public async Task<IEnumerable<Availability>> GetByProviderIncludingInactive(int providerId)
        {
            var sql = @"SELECT * FROM Availability WHERE ProviderId = @ProviderId ORDER BY SpecificDate, DayOfWeek, StartTime";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Availability>(sql, new { ProviderId = providerId });
        }
    }
}
