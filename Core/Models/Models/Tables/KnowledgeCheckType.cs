namespace Models.Models.Tables;

public class KnowledgeCheckType(int id) : TableBase(id)
{
    public string KnowledgeCheckTypeEnum { get; set; } = string.Empty;
}