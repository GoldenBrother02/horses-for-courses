namespace HorsesForCourses.Service;

public class PostCoach
{

    public string Name { get; set; }
    public string Email { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public PostCoach() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public PostCoach(string name, string email)
    {
        Name = name;
        Email = email;
    }
}