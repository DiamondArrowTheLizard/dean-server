namespace Models.Models.Tables;

public class PerformanceDiscipline(int id) : TableBase(id)
{
    public int IdPerformance { get; set; }
    public int IdDiscipline { get; set; }
}