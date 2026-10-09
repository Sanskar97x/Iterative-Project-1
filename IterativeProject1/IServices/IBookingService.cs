using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IterativeProject1.DTOs;

namespace IterativeProject1.Services
{
    public interface IBookingService
    {
        Task<BookingResponseDto> Create(int clientId, CreateBookingDto dto);
       Task<BookingResponseDto> GetById(int bookingId, int requestingUserId, string requestingUserRole);
        Task<IEnumerable<BookingResponseDto>> GetByClient(int clientId);
        Task<IEnumerable<BookingResponseDto>> GetAll();
        Task<bool> Cancel(int bookingId, int requestingUserId, string requestingUserRole);
        Task<BookingResponseDto> Reschedule(int bookingId, int requestingUserId, DateTime newStart, DateTime newEnd);
        Task<bool> UpdateStatus(int bookingId, string status);

    }
}
