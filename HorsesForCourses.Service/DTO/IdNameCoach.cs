namespace HorsesForCourses.Service;

public class IdNameCoach
{
    public int Id { get; set; }
    public string Name { get; set; }

    public IdNameCoach(int id, string name, string email)
    {
        Id = id;
        Name = name;
    }
}