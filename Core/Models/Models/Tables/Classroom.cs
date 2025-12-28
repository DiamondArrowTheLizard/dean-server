namespace Models.Models.Tables;

public class Classroom(int id) : TableBase(id)
{
    public Classroom() : this(0) { }
    public string ClassroomName { get; set; } = string.Empty;
}