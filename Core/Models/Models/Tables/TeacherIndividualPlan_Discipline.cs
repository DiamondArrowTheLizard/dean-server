namespace Models.Models.Tables;

public class TeacherIndividualPlan_Discipline(int id) : TableBase(id)
{
    public int IdTeacherIndividualPlan { get; set; }
    public int IdDiscipline { get; set; }
}