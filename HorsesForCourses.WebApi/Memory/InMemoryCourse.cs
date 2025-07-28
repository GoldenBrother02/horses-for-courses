using HorsesForCourses.Core;

public class InMemoryCourseRepository
{
    private readonly Dictionary<Guid, Course> _courses = new();

    public void Add(Course course)
    {
        _courses[course.Id] = course;
    }

    public Course? GetById(Guid id)
    {
        return _courses.TryGetValue(id, out var coach) ? coach : null;
    }

    public List<CourseDTO> GetAll()
    {
        var list = new List<CourseDTO>();
        foreach (var course in _courses.Values)
        {
            list.Add(new CourseDTO(course.CourseName, course.StartDate, course.EndDate));
        }
        return list;
    }
}