namespace Models.Models.Tables;

public class Classroom(int id) : TableBase(id)
{
    public string ClassroomName { get; set; } = string.Empty;
}