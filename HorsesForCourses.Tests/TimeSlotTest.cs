using HorsesForCourses.Core;

namespace HorsesForCourses.Tests;


public class TimeSlotTest
{
    TimeSlot slot;

    public TimeSlotTest()
    {
        slot = TimeSlot.From(DayOfWeek.Tuesday, new TimeOnly(10, 0), new TimeOnly(11, 0));
    }

    [Fact]
    public void MakeTimeSlot()
    {
        Assert.Equal(DayOfWeek.Tuesday, slot.Day);
        Assert.Equal(new TimeOnly(10, 0), slot.Start);
        Assert.Equal(new TimeOnly(11, 0), slot.End);

        var slot2 = TimeSlot.From(DayOfWeek.Tuesday, new TimeOnly(10, 0), new TimeOnly(11, 0));
        Assert.Equal(slot, slot2);
    }

    [Fact]
    public void BadTimeSlotTest()
    {
        var exception = Assert.Throws<ArgumentException>(() => TimeSlot.From(DayOfWeek.Saturday, new TimeOnly(10), new TimeOnly(11)));
        Assert.Equal("Course cannot take place during the weekend.", exception.Message);

        var exception2 = Assert.Throws<ArgumentException>(() => TimeSlot.From(DayOfWeek.Tuesday, new TimeOnly(7), new TimeOnly(11)));
        Assert.Equal("Course must be planned during working hours (9:00 - 17:00).", exception2.Message);

        var exception3 = Assert.Throws<ArgumentException>(() => TimeSlot.From(DayOfWeek.Tuesday, new TimeOnly(10, 0), new TimeOnly(10, 30)));
        Assert.Equal("Course must be one hour minimum.", exception3.Message);
    }

    [Fact]
    public void OverlapTest()
    {
        var slot2 = TimeSlot.From(DayOfWeek.Tuesday, new TimeOnly(10, 0), new TimeOnly(11, 0));
        var slot3 = TimeSlot.From(DayOfWeek.Wednesday, new TimeOnly(10, 0), new TimeOnly(11, 0));
        var slot4 = TimeSlot.From(DayOfWeek.Tuesday, new TimeOnly(9, 0), new TimeOnly(12, 0));
        var slot5 = TimeSlot.From(DayOfWeek.Tuesday, new TimeOnly(9, 0), new TimeOnly(11, 0));
        var slot6 = TimeSlot.From(DayOfWeek.Tuesday, new TimeOnly(10, 0), new TimeOnly(12, 0));

        Assert.True(slot.Overlap(slot2));
        Assert.False(slot.Overlap(slot3));
        Assert.True(slot.Overlap(slot4));
        Assert.True(slot.Overlap(slot5));
        Assert.True(slot.Overlap(slot6));
    }
}