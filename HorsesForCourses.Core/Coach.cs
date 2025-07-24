using System.Diagnostics.Contracts;

namespace HorsesForCourses.Core;

public class Coach
{
    private List<string> Competencies = new();

    public IReadOnlyList<string> competencies => Competencies;

    private List<TimeSlot> Bookings = new();

    public IReadOnlyList<TimeSlot> bookings => Bookings;

    public string Name { get; set; }

    public EmailAddress Email { get; set; }

    public Guid Id { get; private set; }


    public Coach(string name, string mail)
    {
        Name = name;
        Email = EmailAddress.From(mail);
        Id = Guid.NewGuid();
    }

    public void AddCompetence(string comp)
    {
        if (!Competencies.Contains(comp)) { Competencies.Add(comp); }
        else throw new Exception("Coach already has this competence.");
    }

    public void RemoveCompetence(string comp)
    {
        if (!Competencies.Remove(comp)) { throw new Exception("Coach does not have this competence."); }
    }

    public void BookIn(List<TimeSlot> list)
    {
        if (!bookings.Any(booking => list.Any(slot => booking.Overlap(slot)))) { Bookings.AddRange(list); }
        else throw new Exception("Coach cannot be booked on already booked timeslots.");
    }

    public bool IsCompetent(List<string> requirements)
    {
        var competent = requirements.All(c => Competencies.Contains(c));

        return competent;
    }
}