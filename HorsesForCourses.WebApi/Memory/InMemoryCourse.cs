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

    public List<Course> GetAll()
    {
        return _courses.Values.ToList();
    }
}