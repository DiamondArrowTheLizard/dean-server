namespace Models.Models.Tables;

public class QualificationWork(int id) : TableBase(id)
{
    public string WorkName { get; set; } = string.Empty;
    public int IdTeacher { get; set; }
}