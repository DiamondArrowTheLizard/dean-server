namespace Models.Models.Tables;

public class Curriculum_Performance(int id) : TableBase(id)
{
    public Curriculum_Performance() : this(0) { }
    public int IdCurriculum { get; set; }
    public int IdPerformance { get; set; }
}