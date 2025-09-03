namespace HorsesForCourses.MVC;

public class EditCoachSkills
{
    public int CoachId { get; set; }
    public List<string> Skills { get; set; } = new();
}
