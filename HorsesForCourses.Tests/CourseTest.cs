using HorsesForCourses.Core;

namespace HorsesForCourses.Tests;

public class CourseTest
{
    Course course;
    public CourseTest()
    {
        course = new Course("Appraising Lady Miyabi's magnificence", new DateOnly(2025, 11, 07), new DateOnly(2025, 12, 31));
        var start = new DateOnly(2025, 11, 07);
    }

    [Fact]
    public void MakeCourse()
    {
        Assert.Equal("Appraising Lady Miyabi's magnificence", course.CourseName);
        Assert.Equal(new DateOnly(2025, 11, 07), course.StartDate);
        Assert.Equal(new DateOnly(2025, 12, 31), course.EndDate);
    }

    [Fact]
    public void AddRequirementsTest()
    {
        course.AddRequirement("Eyes");
        Assert.Contains("Eyes", course.RequiredCompetencies);

        var exception = Assert.Throws<Exception>(() => course.AddRequirement("Eyes"));
        Assert.Equal("This required competence is already added.", exception.Message);
    }

    [Fact]
    public void RemoveRequirementTest()
    {
        var exception = Assert.Throws<Exception>(() => course.RemoveRequirement("Eyes"));
        Assert.Equal("This course does not have this requirement.", exception.Message);

        course.AddRequirement("Eyes");
        Assert.Contains("Eyes", course.RequiredCompetencies);

        course.RemoveRequirement("Eyes");
        Assert.Empty(course.RequiredCompetencies);
    }

    [Fact]
    public void AddCourseMomentTest()
    {
        var test = TimeSlot.From(DayOfWeek.Tuesday, new TimeOnly(10, 0), new TimeOnly(11, 0));
        course.AddCourseMoment(test);
        Assert.Contains(test, course.Planning);

        var exception = Assert.Throws<Exception>(() => course.AddCourseMoment(test));
        Assert.Equal("There is overlap between the time slots.", exception.Message);
    }

    [Fact]
    public void RemoveCourseMomentTest()
    {
        var test = TimeSlot.From(DayOfWeek.Tuesday, new TimeOnly(10, 0), new TimeOnly(11, 0));
        var exception = Assert.Throws<Exception>(() => course.RemoveCourseMoment(test));
        Assert.Equal("This is not yet planned in.", exception.Message);

        course.AddCourseMoment(test);
        Assert.Contains(test, course.Planning);

        course.RemoveCourseMoment(test);
        Assert.Empty(course.Planning);
    }

    [Fact]
    public void ConfirmCourseTest()
    {
        var test = TimeSlot.From(DayOfWeek.Tuesday, new TimeOnly(10, 0), new TimeOnly(11, 0));

        var exception = Assert.Throws<Exception>(() => course.ConfirmCourse());
        Assert.Equal("Cannot confirm a course that does not have a planning yet.", exception.Message);

        course.AddCourseMoment(test);
        course.ConfirmCourse();

        var exception2 = Assert.Throws<Exception>(() => course.ConfirmCourse());
        Assert.Equal("Cannot confirm a course that's not in the PENDING state, the current status is CONFIRMED", exception2.Message);
    }

    [Fact]
    public void AddCoachTest()
    {
        var Benny = new Coach("Benny", "Benny@buddy.com");
        var Mark = new Coach("Mark", "Mark@gmail.com");
        var test = TimeSlot.From(DayOfWeek.Tuesday, new TimeOnly(10, 0), new TimeOnly(11, 0));
        var list = new List<TimeSlot>() { test };

        Mark.BookIn(list);
        Mark.AddCompetence("Eyes");
        Mark.AddCompetence("Taste");

        var exception = Assert.Throws<Exception>(() => course.AddCoach(Benny));
        Assert.Equal("Course needs to be CONFIRMED before adding a coach.", exception.Message);

        course.AddRequirement("Eyes");
        course.AddRequirement("Taste");
        course.AddCourseMoment(test);
        course.ConfirmCourse();

        Benny.AddCompetence("Eyes");
        var exception2 = Assert.Throws<Exception>(() => course.AddCoach(Benny));
        Assert.Equal("The coach does not meet the requirements for teaching this course.", exception2.Message);

        Benny.AddCompetence("Taste");

        var exception3 = Assert.Throws<Exception>(() => course.AddCoach(Mark));
        Assert.Equal("Coach is already scheduled in for this time.", exception3.Message);

        course.AddCoach(Benny);
        Assert.Equal(course.coach, Benny);

        var exception4 = Assert.Throws<Exception>(() => course.AddCoach(Benny));
        Assert.Equal("Course has been finalised and cannot be altered.", exception4.Message);



        var FinalException = Assert.Throws<Exception>(() => course.AddCoach(Benny));
        Assert.Equal("Course has been finalised and cannot be altered.", FinalException.Message);

        var exception5 = Assert.Throws<Exception>(() => course.AddRequirement("test"));
        Assert.Equal("Course has been finalised and cannot be altered.", FinalException.Message);

        var exception6 = Assert.Throws<Exception>(() => course.RemoveRequirement("test"));
        Assert.Equal("Course has been finalised and cannot be altered.", FinalException.Message);

        var exception7 = Assert.Throws<Exception>(() => course.AddCourseMoment(test));
        Assert.Equal("Course has been finalised and cannot be altered.", FinalException.Message);

        var exception8 = Assert.Throws<Exception>(() => course.RemoveCourseMoment(test));
        Assert.Equal("Course has been finalised and cannot be altered.", FinalException.Message);
    }
}