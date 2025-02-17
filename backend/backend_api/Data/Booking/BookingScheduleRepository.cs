﻿using System;
using System.Linq;
using System.Threading.Tasks;
using backend_api.Models.Booking;
using backend_api.Models.Booking.Requests;
using backend_api.Models.Booking.Responses;
using Microsoft.EntityFrameworkCore;

namespace backend_api.Data.Booking
{
    public class BookingScheduleRepository: IBookingScheduleRepository
    {
        private readonly BookingScheduleContext _schedules;
        private readonly BookingContext _bookings;
        private readonly object _availabilityLock = new object();


        public BookingScheduleRepository(BookingScheduleContext schedules)
        {
            _schedules = schedules;
        }

        //return a booking schedule for a specific office
        //Not quite sure about how this will be used
        public async Task<GetBookingScheduleResponse> GetBookingSchedule(GetBookingScheduleRequest request)
        {
            var bookingSchedule = await _schedules.BookingSchedules.FindAsync(
                request.Office, request.TimeSlot).AsTask();
            var resp = new GetBookingScheduleResponse(new BookingSchedule(bookingSchedule.TimeSlot, bookingSchedule.Office, bookingSchedule.Availability));
            return resp;
        }

        public async Task<GetAllBookingSchedulesResponse> GetAllBookingSchedules(GetAllBookingSchedulesRequest request)
        {
            var schedules = await _schedules.BookingSchedules
                .OrderByDescending(schedules => schedules.Availability).ToListAsync();
            return new GetAllBookingSchedulesResponse(schedules);
        }

        public async Task<CancelBookingScheduleResponse> CancelBookingSchedule(CancelBookingScheduleRequest request)
        {
            var bookingSchedule = await _schedules.BookingSchedules.FindAsync(
                request.Office, request.TimeSlot).AsTask();
            var scheduleToRemove = new BookingSchedule(bookingSchedule.TimeSlot, bookingSchedule.Office,
                bookingSchedule.Availability);
            try 
            {
                _schedules.BookingSchedules.Remove(scheduleToRemove);
                await _schedules.SaveChangesAsync();
                if (_schedules.Entry(scheduleToRemove).State == EntityState.Deleted)
                    return new CancelBookingScheduleResponse(true);
                else
                {
                    return new CancelBookingScheduleResponse(false);
                }
            }
            catch (Exception)
            {
                return new CancelBookingScheduleResponse(false);
            }
        }
        public async Task<UpdateBookingScheduleResponse> UpdateBookingSchedule(UpdateBookingScheduleRequest request)
        {
            var bookingSchedule = await _schedules.BookingSchedules.FindAsync(
                request.Office, request.TimeSlot).AsTask();
            
            //setting new values
            bookingSchedule.Availability = request.Availability;
            bookingSchedule.Office = request.Office;
            bookingSchedule.TimeSlot = request.TimeSlot;
            try
            {
                await _schedules.SaveChangesAsync();
                return new UpdateBookingScheduleResponse(true);
            }
            catch (Exception)
            {
                return new UpdateBookingScheduleResponse(false);
            }
        }
        public async Task<UpdateBookingScheduleResponse> UpdateBookingScheduleAvailability(UpdateBookingScheduleRequest request)
        {
            var bookingSchedule = await _schedules.BookingSchedules.FindAsync(
                request.Office, request.TimeSlot).AsTask();

            try
            {
                lock (_availabilityLock)
                {
                    bookingSchedule.Availability -= 1;
                }
                await _schedules.SaveChangesAsync();
                return new UpdateBookingScheduleResponse(true);
            }
            catch (Exception)
            {
                return new UpdateBookingScheduleResponse(false);
            }
        }

        public async Task<UpdateBookingScheduleResponse> UpdateBookingScheduleAvailabilityAdd(
            UpdateBookingScheduleRequest request)
        {
            var bookingSchedule = await _schedules.BookingSchedules.FindAsync(
                request.Office, request.TimeSlot).AsTask();

            try
            {
                lock (_availabilityLock)
                {
                    bookingSchedule.Availability += 1;
                }
                await _schedules.SaveChangesAsync();
                return new UpdateBookingScheduleResponse(true);
            }
            catch (Exception)
            {
                return new UpdateBookingScheduleResponse(false);
            }
        }

        public async Task<CreateBookingScheduleResponse> CreateBookingSchedule(CreateBookingScheduleRequest request)
        {
            var bookingSchedule = await _schedules.BookingSchedules.FindAsync(
                request.Office, request.TimeSlot).AsTask();
            
            try
            {
                lock (_availabilityLock)
                {
                    bookingSchedule.Availability = request.Availability;
                }
                
                await _schedules.SaveChangesAsync();
                return new CreateBookingScheduleResponse(true);
            }
            catch (Exception error)
            {
                return new CreateBookingScheduleResponse(false);
            }
        }
        
        
    }
}