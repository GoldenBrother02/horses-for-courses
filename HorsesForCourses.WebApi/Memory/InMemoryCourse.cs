using HorsesForCourses.Core;

public class InMemoryCourseRepository
{
    private readonly Dictionary<int, Course> _courses = new();
    private int NextId = 0;

    public void Add(Course course)
    {
        _courses[course.Id] = course;
    }

    public Course? GetById(int id)
    {
        return _courses.TryGetValue(id, out var coach) ? coach : null;
    }

    public List<GetCourse> GetAll()
    {
        var list = new List<GetCourse>();
        foreach (var course in _courses.Values)
        {
            list.Add(new GetCourse(course.CourseName, course.StartDate, course.EndDate, course.Id, course.Planning.Any(), course.coach is not null));
        }
        return list;
    }

    public int NewId()
    {
        return NextId++;
    }
}