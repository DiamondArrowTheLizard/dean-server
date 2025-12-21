namespace Models.Models.Tables;

public class CurriculumPerformance(int id) : TableBase(id)
{
    public int IdCurriculum { get; set; }
    public int IdPerformance { get; set; }
}