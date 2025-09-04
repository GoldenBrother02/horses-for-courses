using HorsesForCourses.Service;

namespace HorsesForCourses.MVC;

public class EditCourse
{
    public int CourseId { get; set; }
    public List<string> Skills { get; set; } = new();
    public List<TimeSlotDTO> Moments { get; set; } = new();
}