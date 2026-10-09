using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IterativeProject1.DTOs;

namespace IterativeProject1.Services
{
    public interface IAvailabilityService
    {
        Task<int> Create(int providerId, CreateAvailabilityDto dto);
        Task<AvailableSlotsResponseDto> GetOpenSlots(int providerId, DateTime date, int slotDurationMinutes);
        Task<bool> Delete(int id);
        Task<IEnumerable<CreateAvailabilityDto>> GetMine(int providerId);
        Task<DaySlotStatusResponseDto> GetSlotStatuses(int providerId, DateTime date, int slotDurationMinutes, int requestingUserId);
    }
}
