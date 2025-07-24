public class CourseDTO
{
    public string? Name { get; set; }
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }
    public CourseDTO(string name, DateOnly start, DateOnly end)
    {
        Name = name;
        Start = start;
        End = end;
    }
}