using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi;

public class InMemoryCourseRepository
{
    private readonly Dictionary<int, Course> _courses = new();
    private readonly CourseMapper _courseMap;

    public InMemoryCourseRepository(CourseMapper coursemap)
    {
        _courseMap = coursemap;
    }


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
            list.Add(_courseMap.CourseToGetCourse(course));
        }
        return list;
    }

    public int NewId()
    {
        return NextId++;
    }
}