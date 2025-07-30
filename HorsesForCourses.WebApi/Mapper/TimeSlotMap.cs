using HorsesForCourses.Core;

namespace HorsesForCourses.WebApi;

public class TimeSlotMapper
{
    public List<TimeSlot> Map(List<TimeSlotDTO> DTOs)
    {
        var list = new List<TimeSlot>();
        foreach (var slot in DTOs)
        {
            var newslot = new TimeSlot(slot.Day, slot.Start, slot.End);
            list.Add(newslot);
        }
        return list;
    }

    public List<TimeSlotDTO> Revert(List<TimeSlot> List)
    {
        var list = new List<TimeSlotDTO>();
        foreach (var slot in List)
        {
            var newslot = new TimeSlotDTO(slot.Day, slot.Start, slot.End);
            list.Add(newslot);
        }
        return list;
    }
}