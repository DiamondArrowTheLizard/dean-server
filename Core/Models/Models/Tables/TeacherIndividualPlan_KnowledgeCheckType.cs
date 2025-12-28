namespace Models.Models.Tables;

public class TeacherIndividualPlan_KnowledgeCheckType(int id) : TableBase(id)
{
    public TeacherIndividualPlan_KnowledgeCheckType() : this(0) { }
    public int IdTeacherIndividualPlan { get; set; }
    public int IdKnowledgeCheckType { get; set; }
}