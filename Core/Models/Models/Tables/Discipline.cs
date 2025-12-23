namespace Models.Models.Tables;

public class Discipline(int id) : TableBase(id)
{
    public string DisciplineName { get; set; } = string.Empty;
}