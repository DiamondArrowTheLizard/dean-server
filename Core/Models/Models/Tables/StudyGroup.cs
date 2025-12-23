namespace Models.Models.Tables;

public class StudyGroup(int id) : TableBase(id)
{
    public string GroupNumber { get; set; } = string.Empty;
    public int IdCourse { get; set; }
}