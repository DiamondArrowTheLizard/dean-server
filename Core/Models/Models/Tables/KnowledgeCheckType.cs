namespace Models.Models.Tables;

public class KnowledgeCheckType(int id) : TableBase(id)
{
    public KnowledgeCheckType() : this(0) { }
    public string KnowledgeCheckTypeEnum { get; set; } = string.Empty;
}