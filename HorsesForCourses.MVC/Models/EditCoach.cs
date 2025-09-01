namespace HorsesForCourses.MVC;

public class EditCoachSkills
{
    public int CoachId { get; set; }
    public List<string> CurrentSkills { get; set; } = new();
    public List<string> NewSkills { get; set; } = new();
}
