public class CoachDTO
{

    public string? Name { get; set; }
    public string? Email { get; set; }
    public List<string> Competencies = new();

    public CoachDTO(string name, string mail, List<string> competencies)
    {
        Name = name;
        Email = mail;
        Competencies = competencies;
    }
}