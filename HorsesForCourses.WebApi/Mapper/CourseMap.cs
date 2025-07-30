using HorsesForCourses.Core;

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
}