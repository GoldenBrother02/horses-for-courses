using HorsesForCourses.Core;

namespace HorsesForCourses.Tests;

public class CoachTest
{
    List<TimeSlot> planning = new();
    Coach Benny;
    public CoachTest()
    {
        Benny = new Coach("Benny", "Benny@buddy.com");
        var time = new TimeSlot(DayOfWeek.Tuesday, new TimeOnly(10), new TimeOnly(11));
        var time2 = new TimeSlot(DayOfWeek.Monday, new TimeOnly(10), new TimeOnly(11));
        var time3 = new TimeSlot(DayOfWeek.Wednesday, new TimeOnly(10), new TimeOnly(11));
        var time4 = new TimeSlot(DayOfWeek.Thursday, new TimeOnly(10), new TimeOnly(11));
        var time5 = new TimeSlot(DayOfWeek.Friday, new TimeOnly(10), new TimeOnly(11));

        planning.Add(time);
        planning.Add(time2);
        planning.Add(time3);
        planning.Add(time4);
        planning.Add(time5);
    }

    [Fact]
    public void MakeCoach()
    {
        Assert.Equal("Benny", Benny.Name);
        Assert.Equal(EmailAddress.From("Benny@buddy.com"), Benny.Email);
    }

    [Fact]
    public void AddCompetenceTest()
    {
        Benny.AddCompetence("Javascript");
        Assert.Contains("Javascript", Benny.competencies);
    }

    [Fact]
    public void AddCompetenceFailTest()
    {
        Benny.AddCompetence("Javascript");
        var exception = Assert.Throws<Exception>(() => Benny.AddCompetence("Javascript"));
        Assert.Equal("Coach already has this competence.", exception.Message);
    }

    [Fact]
    public void RemoveCompetenceTest()
    {
        Benny.AddCompetence("Javascript");
        Assert.Contains("Javascript", Benny.competencies);

        Benny.RemoveCompetence("Javascript");
        Assert.DoesNotContain("Javascript", Benny.competencies);
    }

    [Fact]
    public void RemoveCompetenceFailTest()
    {
        var exception = Assert.Throws<Exception>(() => Benny.RemoveCompetence("Javascript"));
        Assert.Equal("Coach does not have this competence.", exception.Message);
    }

    [Fact]
    public void BookInTest()
    {
        Benny.BookIn(planning);

        Assert.Equal(Benny.bookings, planning);
    }

    [Fact]
    public void BookInFailTest()
    {
        Benny.BookIn(planning);

        var exception = Assert.Throws<Exception>(() => Benny.BookIn(planning));
        Assert.Equal("Coach cannot be booked on already booked timeslots.", exception.Message);
    }

    [Fact]
    public void IsCompetentTest()
    {
        var comp = new List<string>() { "Javascript" };
        var comp2 = new List<string>() { "Javascript", "De samba" };
        Benny.AddCompetence("Javascript");

        Assert.True(Benny.IsCompetent(comp));
        Assert.False(Benny.IsCompetent(comp2));
    }
}