using System.Linq.Expressions;

namespace HorsesForCourses.Core;

public record Booking
{
    public List<TimeSlot> Planning { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Booking() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public Booking(List<TimeSlot> planning, DateOnly startdate, DateOnly enddate) { Planning = planning; StartDate = startdate; EndDate = enddate; }


    public static Booking From(List<TimeSlot> planning, DateOnly startdate, DateOnly enddate)
    {
        if (startdate > enddate) throw new Exception("start date must be before end date");
        bool x = startdate.Year == enddate.Year;
        bool y = startdate.Month == enddate.Month;
        bool z = (enddate.DayNumber - startdate.DayNumber) < 7;
        if (x && y && z)
        {
            foreach (var slot in planning)
            {
                if (slot.Day > enddate.DayOfWeek || slot.Day < startdate.DayOfWeek) { throw new Exception("Day of timeslot should appear in duration of booking."); }
            }
        }
        return new Booking(planning, startdate, enddate);
    }


    private bool PeriodOverlap(Booking booking)
    {
        return (booking.StartDate < StartDate && booking.EndDate > StartDate)
            || (booking.StartDate > StartDate && booking.EndDate < EndDate)
            || (booking.StartDate < EndDate && booking.EndDate > EndDate)
            || (booking.StartDate <= EndDate && booking.EndDate > StartDate)
            || (booking.StartDate == StartDate && booking.EndDate == EndDate);
    }

    private bool PlanningOverlap(Booking booking)
    {
        return Planning.Any(slot => booking.Planning.Any(newslot => slot.Overlap(newslot)));
    }

    public bool BookingOverlap(Booking booking)
    {
        if (PeriodOverlap(booking))
        {
            return PlanningOverlap(booking);
        }
        else
        {
            return false;
        }
    }

}