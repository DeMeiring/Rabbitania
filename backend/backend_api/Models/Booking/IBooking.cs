﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using backend_api.Models.User;

namespace backend_api.Models.Booking
{
    public interface IBooking
    {
         int BookingId { get; set; }
        
         DateTime BookingDate { get; set; }
         
         OfficeLocation Office { get; set; }
         
         public string TimeSlot { get; set; }

         int UserId { get; set; }
         
    }
}