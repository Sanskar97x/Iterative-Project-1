using System.Collections.Generic;
using System.Threading.Tasks;
using IterativeProject1.Models;

namespace IterativeProject1.Repositories
{
    public interface IAvailabilityRepository
    {
        Task<int> Create(Availability availability);
        Task<IEnumerable<Availability>> GetByProvider(int providerId);
        Task<Availability> GetById(int id);
        Task<IEnumerable<Availability>> GetByProviderIncludingInactive(int providerId);
        Task<bool> Delete(int id);
    }
}