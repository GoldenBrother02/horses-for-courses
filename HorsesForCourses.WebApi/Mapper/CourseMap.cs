using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi;

public class CourseMapper
{
    public Course DTOToCourse(CourseDTO DTO)
    {
        return new Course(DTO.Id, DTO.Name, DTO.Start, DTO.End);
    }
    public List<IdNameCourse> ListToIdName(List<Course> List)
    {
        var list = new List<IdNameCourse>();
        foreach (var course in List)
        {
            list.Add(new IdNameCourse(course.Id, course.CourseName));
        }
        return list;
    }

    public Course PostToCourse(PostCourse DTO, int id)
    {
        return new Course(id, DTO.Name, DTO.Start, DTO.End);
    }

    public GetCourse CourseToGetCourse(Course course)
    {
        return new GetCourse(course.CourseName, course.StartDate, course.EndDate, course.Id, course.Planning.Any(), course.coach is not null);
    }

    public int GetNextId(AppDbContext context)
    {
        return context.Courses.Any() ? context.Courses.Max(c => c.Id) : 0;
    }
}