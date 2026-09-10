using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IterativeProject1.DTOs;
using IterativeProject1.Helpers;
using IterativeProject1.Models;
using IterativeProject1.Repositories;


namespace IterativeProject1.Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IAvailabilityRepository _availabilityRepository;
        private readonly IBookingRepository _bookingRepository;

        public AvailabilityService(IAvailabilityRepository availabilityRepository, IBookingRepository bookingRepository)
        {
            _availabilityRepository = availabilityRepository;
            _bookingRepository = bookingRepository;
        }

         public async Task<int> Create(int providerId, CreateAvailabilityDto dto)
         {
             var availability = new Availability
             {
                    ProviderId = providerId,
                    DayOfWeek = dto.DayOfWeek,
                    SpecificDate = dto.SpecificDate,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    IsActive = true
             };
                return await _availabilityRepository.Create(availability);
            }

        public async Task<AvailableSlotsResponseDto> GetOpenSlots(int providerId, DateTime date, int slotDurationMinutes)
        {
            var allAvailabilities = (await _availabilityRepository.GetByProviderIncludingInactive(providerId))
                .Where(a => a.SpecificDate == date.Date || a.DayOfWeek == (byte)date.DayOfWeek);

            var availabilities = allAvailabilities.Where(a => a.IsActive);

            var dayStart = date.Date;
            var dayEnd = date.Date.AddDays(1);
            var existingBookings = await _bookingRepository.GetByProviderAndDateRange(providerId, dayStart, dayEnd);

            var slots = TimeSlotHelper.GenerateOpenSlots(date, availabilities, existingBookings, slotDurationMinutes);

            string message;
            if (!allAvailabilities.Any())
                message = "No availability has been set for this provider on this date.";
            else if (!availabilities.Any())
                message = "Availability exists for this date but has been deactivated.";
            else if (slots.Count == 0)
                message = "Availability exists but all slots are already booked.";
            else
                message = "Slots available.";

            return new AvailableSlotsResponseDto
            {
                ProviderId = providerId,
                Date = date,
                SlotDurationMinutes = slotDurationMinutes,
                TotalAvailabilityWindowsFound = availabilities.Count(),
                InactiveAvailabilityWindowsFound = allAvailabilities.Count() - availabilities.Count(),
                TotalOpenSlots = slots.Count,
                Message = message,
                Slots = slots.Select(s => new AvailabilitySlotDto
                {
                    ProviderId = providerId,
                    SlotStart = s.Start,
                    SlotEnd = s.End
                }).ToList()
            };
        }

        public async Task<bool> Delete(int id)
        {
            return await _availabilityRepository.Delete(id);
        }

        }
    }




