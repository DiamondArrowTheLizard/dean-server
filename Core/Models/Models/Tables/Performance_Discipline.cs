namespace Models.Models.Tables;

public class Performance_Discipline(int id) : TableBase(id)
{
    public Performance_Discipline() : this(0) { }
    public int IdPerformance { get; set; }
    public int IdDiscipline { get; set; }
}