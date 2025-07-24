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
/*
public Course(string name, DateOnly start, DateOnly end)
    {
        CourseName = name;
        Status = States.PENDING;
        Planning = new();
        RequiredCompetencies = new();
        coach = null;
        StartDate = start;
        EndDate = end;
        Id = Guid.NewGuid();
    }*/