namespace HorsesForCourses.WebApi;

public class CoachDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<string>? Competencies { get; set; } = new();
    public List<IdNameCourse> CourseList { get; set; }

    public CoachDTO(int id, string name, string email, List<string> competencies, List<IdNameCourse> courseList)
    {
        Id = id;
        Name = name;
        Email = email;
        Competencies = competencies;
        CourseList = courseList;
    }
}