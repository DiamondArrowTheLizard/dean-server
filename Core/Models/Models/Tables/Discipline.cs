namespace Models.Models.Tables;

public class Discipline(int id) : TableBase(id)
{
    public Discipline() : this(0) { }
    public string DisciplineName { get; set; } = string.Empty;
}