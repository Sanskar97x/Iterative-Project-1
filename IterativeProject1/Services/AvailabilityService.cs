using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IterativeProject1.DTOs;
using IterativeProject1.Repositories;
using IterativeProject1.Models;
using IterativeProject1.Helpers;


namespace IterativeProject1.Services
{
    public class AvailabilityService  // : IAvailabilityService
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

        public async Task<IEnumerable<AvailabilitySlotDto>>GetOpenSlots(int providerId, DateTime date, int slotDurationMinutes)
        {
            var availabilities = (await _availabilityRepository.GetByProvider(providerId)).Where(a => a.SpecificDate == date.Date || a.DayOfWeek == (byte)date.DayOfWeek);

            var dayStart = date.Date;
            var dayEnd = date.Date.AddDays(1);
            var existingBookings = await _bookingRepository.GetByProviderAndDateRange(providerId, dayStart, dayEnd);

            var slots = TimeSlotHelper.GenerateOpenSlots(date, availabilities, existingBookings, slotDurationMinutes);

            return slots.Select(s => new AvailabilitySlotDto
            {
                ProviderId = providerId,
                SlotStart = s.Start,
                SlotEnd = s.End
            });
        }

        public async Task<bool> Delete(int id)
        {
            return await _availabilityRepository.Delete(id);
        }

        }
    }




