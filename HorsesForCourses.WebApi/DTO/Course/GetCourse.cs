public class GetCourse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }
    public bool HasSchedule { get; set; }
    public bool HasCoach { get; set; }
    public GetCourse(string name, DateOnly start, DateOnly end, int id, bool schedule, bool coach)
    {
        Id = id;
        Name = name;
        Start = start;
        End = end;
        HasSchedule = schedule;
        HasCoach = coach;
    }
}