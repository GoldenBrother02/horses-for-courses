public class GetCoach
{

    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int NumberOfCoursesAssignedTo { get; set; }

    public GetCoach(int id, string name, string email, int courseTotal)
    {
        Id = id;
        Name = name;
        Email = email;
        NumberOfCoursesAssignedTo = courseTotal;
    }
}