﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using backend_api.Data.Booking;
using backend_api.Data.User;
using backend_api.Exceptions.Booking;
using backend_api.Models.Auth.Requests;
using backend_api.Models.Booking.Requests;
using backend_api.Models.Booking.Responses;
using backend_api.Models.Enumerations;
using backend_api.Models.User;
using backend_api.Services.Auth;
using backend_api.Services.Booking;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace backend_api.Tests.Booking.Integration
{
    public class BookingServiceTests
    {
        private readonly Models.Booking.Booking _mockBooking;
        private readonly BookingContext _bookingContext;
        private readonly BookingScheduleContext _bookingSchedule;
        public BookingServiceTests()
        {
            var serviceProvider = new ServiceCollection().AddEntityFrameworkNpgsql().BuildServiceProvider();
            var builder = new DbContextOptionsBuilder<BookingContext>();
            var builder1 = new DbContextOptionsBuilder<BookingScheduleContext>();
            
            var env = Environment.GetEnvironmentVariable("CONN_STRING");
            builder.UseNpgsql(env.ToString())
                .UseInternalServiceProvider(serviceProvider);
            
            _bookingContext = new BookingContext(builder.Options);
            _bookingSchedule = new BookingScheduleContext(builder1.Options);
            
             this._mockBooking = new Models.Booking.Booking(
                 "2021-08-17 19:58",
                "Monday,Morning",
                0,
                1
            );
        }
        
        [Fact(DisplayName = "Should return created status code if a new booking is created with correct details")]
        public async void ShouldCreateANewBooking()
        {
            //Arrange
            var bookingRepository = new BookingRepository(_bookingContext);
            var bookingScheduleRepo = new BookingScheduleRepository(_bookingSchedule);
            var bookingScheduleServ = new BookingScheduleService(bookingScheduleRepo);
            
            var bookingService = new BookingService(bookingRepository);

            var requestDto = new CreateBookingRequest
            {
                BookingDate = _mockBooking.BookingDate,
                TimeSlot = _mockBooking.TimeSlot,
                Office = _mockBooking.Office,
                UserId = _mockBooking.UserId
            };

            //Act
            var actualResponse = await bookingRepository.CreateBooking(requestDto);
            /*_bookingContext.Bookings.Remove(_mockBooking);
            await _bookingContext.SaveChanges();*/
    
            //Assert
            actualResponse.Should().Be(HttpStatusCode.Created);
        }

        [Fact(DisplayName = "Should return a bad request if details are not correct")]
        public async void ShouldNotCreateANewBooking()
        {
            //Arrange
            var bookingRepository = new BookingRepository(_bookingContext);
            var bookingScheduleRepo = new BookingScheduleRepository(_bookingSchedule);
            var bookingScheduleServ = new BookingScheduleService(bookingScheduleRepo);
            
            var bookingService = new BookingService(bookingRepository);

            var requestDto = new CreateBookingRequest
            {
                BookingDate = "",
                TimeSlot = "",
                Office = 0,
                UserId = -1
            };

            //Act
            var actualResponse = await bookingRepository.CreateBooking(requestDto);

            //Assert
            actualResponse.Should().Be(HttpStatusCode.BadRequest);
        }
        
        [Fact(DisplayName = "Should return all bookings that have been created")]
        public async void ShouldReturnAllBookingsCurrently()
        {
            //Arrange
            var bookingRepository = new BookingRepository(_bookingContext);
            var bookingService = new BookingService(bookingRepository);
            
            var requestDto = new GetAllBookingsRequest()
            {
                UserId = 1
            };
            var expected = await bookingRepository.GetAllBookings(requestDto);
            
            //Act
            var actualResponse = await bookingService.ViewAllBookings(requestDto);

            //Assert
            var @equals = actualResponse.Bookings.Equals(expected);
        }
        
        [Fact(DisplayName = "Should not return all bookings that have been created")]
        public async void ShouldNotReturnAllBookingsCurrently()
        {
            //Arrange
            var bookingRepository = new BookingRepository(_bookingContext);
            var bookingService = new BookingService(bookingRepository);
            
            var requestDto = new GetAllBookingsRequest()
            {
                UserId = 0
            };
            var expected = await bookingRepository.GetAllBookings(requestDto);
            
            //Act
            var actualResponse = await bookingService.ViewAllBookings(requestDto);
            
            //Assert
            Assert.Empty(actualResponse.Bookings);
        }
        
        [Fact(DisplayName = "Should check availability for a booking and should pass of there is availabilities.")]
        public async void ShouldDeleteABookingAndReturnAccepted()
        {
            //Arrange
            var bookingRepository = new BookingRepository(_bookingContext);
            var bookingScheduleRepository = new BookingScheduleRepository(_bookingSchedule);
            var bookingScheduleService = new BookingScheduleService(bookingScheduleRepository);
            
            var bookingService = new BookingService(bookingRepository, bookingScheduleRepository, bookingScheduleService);

            var requestDto = new CancelBookingRequest()
            {
                BookingId = 1
            };
            
            var expected = bookingRepository.CancelBooking(requestDto).Result;
            
            //Act
            var actualResponse = await bookingRepository.CancelBooking(requestDto);
            
            //Assert
            Assert.Equal(expected, actualResponse);
        } 
        
        [Fact(DisplayName = "Should not delete a booking from the test database and return accepted.")]
        public async void ShouldNotDeleteABookingAndReturnAccepted()
        {
            //Arrange
            var bookingRepository = new BookingRepository(_bookingContext);
            var bookingScheduleRepository = new BookingScheduleRepository(_bookingSchedule);
            var bookingScheduleService = new BookingScheduleService(bookingScheduleRepository);
            
            var bookingService = new BookingService(bookingRepository, bookingScheduleRepository, bookingScheduleService);

            var requestDto = new CancelBookingRequest()
            {
                BookingId = 0
            };
            
            var expected = bookingRepository.CancelBooking(requestDto).Result;
            
            //Act
            var actualResponse = await bookingRepository.CancelBooking(requestDto);
            
            //Assert
            Assert.Equal(expected, actualResponse);
        }
        
        [Fact(DisplayName = "Should check availability for a booking and should pass if there are availabilities.")]
        public async void Should_Check_For_Availabilities_And_Pass_If_A_Schedule_Has_A_Availability()
        {
            //Arrange
            var bookingRepository = new BookingRepository(_bookingContext);
            var bookingScheduleRepository = new BookingScheduleRepository(_bookingSchedule);
            var bookingScheduleService = new BookingScheduleService(bookingScheduleRepository);
            
            var bookingService = new BookingService(bookingRepository, bookingScheduleRepository, bookingScheduleService);

            var requestDto = new CheckIfBookingExistsRequest(
                "Monday,Afternoon", 
                OfficeLocation.Pretoria, 
                1
            );

            //Act
            var actualResponse = await bookingService.CheckIfBookingExists(requestDto);
            
            //Assert
            Assert.Equal(HttpStatusCode.Accepted, actualResponse.Response);
        } 
    }
}