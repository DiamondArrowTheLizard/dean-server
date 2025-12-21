namespace Models.Models.Tables;

public class Faculty(int id) : TableBase(id)
{
    public string FacultyName { get; set; } = string.Empty;
}