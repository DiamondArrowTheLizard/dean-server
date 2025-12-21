namespace Models.Models.Tables;

public class TeacherIndividualPlanKnowledgeCheckType(int id) : TableBase(id)
{
    public int IdTeacherIndividualPlan { get; set; }
    public int IdKnowledgeCheckType { get; set; }
}