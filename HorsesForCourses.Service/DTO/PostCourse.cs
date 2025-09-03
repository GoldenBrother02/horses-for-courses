namespace HorsesForCourses.Service;

public class PostCourse
{
    public string Name { get; set; }
    public DateOnly Start { get; set; }
    public DateOnly End { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public PostCourse() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public PostCourse(string name, DateOnly start, DateOnly end)
    {
        Name = name;
        Start = start;
        End = end;
    }
}