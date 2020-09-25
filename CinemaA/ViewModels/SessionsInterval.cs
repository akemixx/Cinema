using CinemaA.Models;
using System;
using System.ComponentModel.DataAnnotations;

/*
 * Used in Sessions Controller
 */

namespace CinemaA.ViewModels
{
    public class SessionsInterval
    {
        // General information about session.
        public Session Session { get; set; }
        [DataType(DataType.Date)]
        // Start date for sessions set.
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        // End date for sessions set.
        public DateTime EndDate { get; set; }
    }
}
