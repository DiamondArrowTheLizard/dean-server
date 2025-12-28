namespace Models.Models.Tables;

public class QualificationWork(int id) : TableBase(id)
{
    public QualificationWork() : this(0) { }
    public string WorkName { get; set; } = string.Empty;
    public int IdTeacher { get; set; }
}