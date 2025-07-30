using System.Reflection.Metadata.Ecma335;
using HorsesForCourses.Core;
using Microsoft.AspNetCore.Http.HttpResults;

public class InMemoryCoachRepository
{
    private readonly Dictionary<int, Coach> _coaches = new();
    private int NextId = 0;

    public void Add(Coach coach)
    {
        _coaches[coach.Id] = coach;
    }

    public Coach? GetById(int id)
    {
        return _coaches.TryGetValue(id, out var coach) ? coach : null;
    }

    public List<GetCoach> GetAll()
    {
        var list = new List<GetCoach>();
        foreach (var coach in _coaches.Values)
        {
            list.Add(new GetCoach(coach.Id, coach.Name, coach.Email.ToString(), coach.CourseList.Count()));
        }
        return list;
    }

    public int NewId()
    {
        return NextId++;
    }
}