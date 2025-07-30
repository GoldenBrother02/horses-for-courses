namespace HorsesForCourses.WebApi;

public class IdNameCourse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IdNameCourse(int id, string name)
    {
        Id = id;
        Name = name;
    }
}