using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IterativeProject1.Data;
using Dapper;
using IterativeProject1.Models;

namespace IterativeProject1.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly DapperContext _context;

        public BookingRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<int> Create(Booking booking)
        {
            var sql = @"INSERT INTO Bookings (ClientId, ProviderId, StartTime, EndTime, Status, Notes, CreatedAt)
                        VALUES (@ClientId, @ProviderId, @StartTime, @EndTime, @Status, @Notes, GETUTCDATE());
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(sql, booking);
        }

        public async Task<Booking> GetById(int id)
        {
            var sql = @"SELECT * FROM Bookings WHERE Id = @Id";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Booking>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Booking>> GetByClient(int clientId)
        {
            var sql = @"SELECT * FROM Bookings WHERE ClientId = @ClientId ORDER BY StartTime DESC";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Booking>(sql, new { ClientId = clientId });
        }

        public async Task<IEnumerable<Booking>> GetAll()
        {
            var sql = @"SELECT * FROM Bookings ORDER BY StartTime DESC";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Booking>(sql);
        }

        public async Task<IEnumerable<Booking>> GetByProviderAndDateRange(int providerId, DateTime from, DateTime to)
        {
            var sql = @"SELECT * FROM Bookings 
                        WHERE ProviderId = @ProviderId 
                          AND Status != 'Cancelled'
                          AND StartTime < @To 
                          AND EndTime > @From
                        ORDER BY StartTime";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Booking>(sql, new { ProviderId = providerId, From = from, To = to });
        }

        public async Task<bool> HasConflict(int providerId, DateTime start, DateTime end, int? excludeBookingId = null)
        {
            var sql = @"SELECT COUNT(1) FROM Bookings 
                        WHERE ProviderId = @ProviderId 
                          AND Status != 'Cancelled'
                          AND StartTime < @End 
                          AND EndTime > @Start
                          AND (@ExcludeBookingId IS NULL OR Id != @ExcludeBookingId)";
            using var connection = _context.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(sql, new
            {
                ProviderId = providerId,
                Start = start,
                End = end,
                ExcludeBookingId = excludeBookingId
            });
            return count > 0;
        }

        //Find the booking with this Id and change its Status.
        public async Task<bool> UpdateStatus(int id, string status)
        {
            var sql = @"UPDATE Bookings SET Status = @Status, UpdatedAt = GETUTCDATE() WHERE Id = @Id";
            using var connection = _context.CreateConnection();
            var rows = await connection.ExecuteAsync(sql, new { Id = id, Status = status });
            return rows > 0;
        }
    }
}


