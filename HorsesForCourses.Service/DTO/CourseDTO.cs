using HorsesForCourses.Core;

namespace HorsesForCourses.Service;

public class CourseDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }
    public List<string> Competenties { get; set; }
    public List<TimeSlotDTO> TimeSlots { get; set; }
    public IdNameCoach? Coach { get; set; }
    public States Status { get; set; }
    public CourseDTO(string name, DateOnly start, DateOnly end, int id, States status, List<string>? competenties = null,
        List<TimeSlotDTO>? timeSlots = null,
        IdNameCoach? coach = null)
    {
        Id = id;
        Name = name;
        Start = start;
        End = end;
        Competenties = competenties ?? new List<string>();
        TimeSlots = timeSlots ?? new List<TimeSlotDTO>();
        Coach = coach;
        Status = status;
    }
}