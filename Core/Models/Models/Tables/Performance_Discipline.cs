namespace Models.Models.Tables;

public class Performance_Discipline(int id) : TableBase(id)
{
    public int IdPerformance { get; set; }
    public int IdDiscipline { get; set; }
}