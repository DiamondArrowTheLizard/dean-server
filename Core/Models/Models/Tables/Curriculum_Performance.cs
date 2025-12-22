namespace Models.Models.Tables;

public class Curriculum_Performance(int id) : TableBase(id)
{
    public int IdCurriculum { get; set; }
    public int IdPerformance { get; set; }
}