using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi;

public class CoachMapper
{
    public Coach Map(PostCoach DTO, int Id)
    {
        return new Coach(Id, DTO.Name, DTO.Email);
    }

    public IdNameCoach CoachToIdName(Coach coach)
    {
        if (coach is null) { return null!; }
        return new IdNameCoach(coach.Id, coach.Name, coach.Email.ToString());
    }

    public GetCoach CoachToGetCoach(Coach coach)
    {
        return new GetCoach(coach.Id, coach.Name, coach.Email.ToString(), coach.CourseList.Count());
    }
}