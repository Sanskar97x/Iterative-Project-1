using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IterativeProject1.Models;

namespace IterativeProject1.Repositories
{
    public interface IBookingRepository
    {
        Task<int> Create(Booking booking);
        Task<Booking> GetById(int id);
        Task<IEnumerable<Booking>> GetByClient(int clientId);
        Task<IEnumerable<Booking>> GetAll();
        Task<IEnumerable<Booking>> GetByProviderAndDateRange(int providerId, DateTime from, DateTime to);
        Task<bool> HasConflict(int providerId, DateTime start, DateTime end, int? excludeBookingId = null);
        Task<bool> UpdateStatus(int id, string status);
    }
}