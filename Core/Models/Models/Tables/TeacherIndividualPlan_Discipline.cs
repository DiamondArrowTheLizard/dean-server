namespace Models.Models.Tables;

public class TeacherIndividualPlanDiscipline(int id) : TableBase(id)
{
    public int IdTeacherIndividualPlan { get; set; }
    public int IdDiscipline { get; set; }
}