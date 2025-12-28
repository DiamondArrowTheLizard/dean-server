namespace Models.Models.Tables;

public class Faculty(int id) : TableBase(id)
{
    public Faculty() : this(0) { }

    public string FacultyName { get; set; } = string.Empty;
}