using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IterativeProject1.DTOs;
using IterativeProject1.Repositories;
using IterativeProject1.Models;

namespace IterativeProject1.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }
        private static BookingResponseDto MapToDto(Booking booking)
        {
            return new BookingResponseDto
            {
                Id = booking.Id,
                ClientId = booking.ClientId,
                ProviderId = booking.ProviderId,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Status = booking.Status,
                Notes = booking.Notes
            };
        }

        public async Task<BookingResponseDto> Create(int clientId, CreateBookingDto dto)
        {

            if (dto.StartTime < DateTime.UtcNow)
                throw new InvalidOperationException("Cannot book a time in the past");

            var conflict = await _bookingRepository.HasConflict(dto.ProviderId, dto.StartTime, dto.EndTime);
            if (conflict)
                throw new InvalidOperationException("This time slot is no longer available.");

            var booking = new Booking
            {
                ClientId = clientId,
                ProviderId = dto.ProviderId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = "Pending",
                Notes = dto.Notes
            };

            var id = await _bookingRepository.Create(booking);
            var created = await _bookingRepository.GetById(id);

            return MapToDto(created);
        }

       public async Task<BookingResponseDto> GetById(int bookingId, int requestingUserId, string requestingUserRole)
        {
            var booking = await _bookingRepository.GetById(bookingId);
            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            bool isOwner = booking.ClientId == requestingUserId;
            bool isAdmin = requestingUserRole == "Admin";

            if (!isOwner && !isAdmin)
                throw new UnauthorizedAccessException("You cannot view this booking.");

            return MapToDto(booking);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetByClient(int clientId)
        {
            var bookings = await _bookingRepository.GetByClient(clientId);
            return bookings.Select(MapToDto);
        }

        public async Task<IEnumerable<BookingResponseDto>> GetAll()
        {
            var bookings = await _bookingRepository.GetAll();
            return bookings.Select(MapToDto);
        }
        public async Task<bool> Cancel(int bookingId, int requestingUserId, string requestingUserRole)
        {
            var booking = await _bookingRepository.GetById(bookingId);
            if (booking == null)
                throw new KeyNotFoundException("Booking not found.");

            bool isOwner = booking.ClientId == requestingUserId;
            bool isAdmin = requestingUserRole == "Admin";

            if (!isOwner && !isAdmin)
                throw new UnauthorizedAccessException("You cannot cancel this booking");

            return await _bookingRepository.UpdateStatus(bookingId,"Cancelled");
        }

        public async Task<BookingResponseDto> Reschedule(int bookingId, int requestingUserId, DateTime newStart, DateTime newEnd)
        {
            var booking = await _bookingRepository.GetById(bookingId);

            if (booking == null)
                throw new KeyNotFoundException("Booking Not Found.");

            if (booking.ClientId != requestingUserId)
                throw new UnauthorizedAccessException("You cannot reschedule this booking");

            var conflict = await _bookingRepository.HasConflict(booking.ProviderId, newStart, newEnd, excludeBookingId: bookingId);
            if (conflict)
                throw new InvalidOperationException("The new time slot is not available");

            await _bookingRepository.UpdateStatus(bookingId, "Cancelled");

            var newBooking = new Booking
            {
                ClientId = booking.ClientId,
                ProviderId = booking.ProviderId,
                StartTime = newStart,
                EndTime = newEnd,
                Notes = booking.Notes,
                Status = "Pending"
            };
            var newId = await _bookingRepository.Create(newBooking);
            var created = await _bookingRepository.GetById(newId);

            return MapToDto(created);
        }

        public async Task<bool> UpdateStatus(int bookingId, string status)
        {
            //creating an array containing the statuses that your application considers valid.
            var validStatuses = new[] { "Pending", "Confirmed", "Cancelled", "Completed" };

            //Contains() is a method that checks whether a collection(validStatus) contains a specific value.
            if (!validStatuses.Contains(status))
                throw new ArgumentException("Invalid status value.");

            return await _bookingRepository.UpdateStatus(bookingId, status);


        }
 }
}


