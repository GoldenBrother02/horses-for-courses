namespace HorsesForCourses.WebApi;

public class CourseDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }
    public List<string> Competenties { get; set; }
    public List<TimeSlotDTO> TimeSlots { get; set; }
    public IdNameCoach? Coach { get; set; }
    public CourseDTO(string name, DateOnly start, DateOnly end, int id, List<string> competenties, List<TimeSlotDTO> timeSlots, IdNameCoach coach)
    {
        Id = id;
        Name = name;
        Start = start;
        End = end;
        Competenties = competenties;
        TimeSlots = timeSlots;
        Coach = coach;
    }
}