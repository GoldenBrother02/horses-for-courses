public class CoachDTO
{

    public string? Name { get; set; }
    public string? Email { get; set; }
    public List<string>? Competencies { get; set; } = new();

    public CoachDTO(string name, string email, List<string> competencies)
    {
        Name = name;
        Email = email;
        Competencies = competencies;
    }
}