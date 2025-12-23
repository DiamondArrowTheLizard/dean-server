namespace Models.Models.Tables;

public class Performance_KnowledgeCheckType(int id) : TableBase(id)
{
    public int IdPerformance { get; set; }
    public int IdKnowledgeCheckType { get; set; }
}