namespace HorsesForCourses.MVC;

public class AddCoachToCourse
{
    public int CourseId { get; set; }
    public int CoachId { get; set; } = new();
}