namespace Models.Models.Tables;

public class TeacherIndividualPlan_KnowledgeCheckType(int id) : TableBase(id)
{
    public int IdTeacherIndividualPlan { get; set; }
    public int IdKnowledgeCheckType { get; set; }
}