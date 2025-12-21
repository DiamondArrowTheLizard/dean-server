namespace Models.Models.Tables;

public class PerformanceKnowledgeCheckType(int id) : TableBase(id)
{
    public int IdPerformance { get; set; }
    public int IdKnowledgeCheckType { get; set; }
}