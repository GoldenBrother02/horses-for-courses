public class PostCourse
{
    public string Name { get; set; }
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }
    public PostCourse(string name, DateOnly start, DateOnly end)
    {
        Name = name;
        Start = start;
        End = end;
    }
}