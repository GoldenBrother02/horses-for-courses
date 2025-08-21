using HorsesForCourses.Core;

namespace HorsesForCourses.Tests;

public class BookingEdges
{
    [Fact]
    public void Overlaps_on_friday_but_we_only_teach_on_thursday()
    {
        // probably better guarded against/handled on Booking creation 
        var bookingOne = Booking.From([TimeSlot.From(DayOfWeek.Friday, new TimeOnly(9, 0), new TimeOnly(10, 0))],
            new DateOnly(2025, 8, 21),
            new DateOnly(2025, 8, 21));

        var bookingTwo = Booking.From([TimeSlot.From(DayOfWeek.Friday, new TimeOnly(9, 0), new TimeOnly(10, 0))],
            new DateOnly(2025, 8, 21),
            new DateOnly(2025, 8, 21));

        Assert.Equal(DayOfWeek.Thursday, new DateOnly(2025, 8, 21).DayOfWeek);
        Assert.False(bookingOne.BookingOverlap(bookingTwo), "Bookings should not overlap.");
    }
}
