namespace Models.Models.Tables;

public class Course(int id) : TableBase(id)
{
    public Course() : this(0) { }
    public string Code { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string Profile { get; set; } = string.Empty;
    public int IdFaculty { get; set; }
}