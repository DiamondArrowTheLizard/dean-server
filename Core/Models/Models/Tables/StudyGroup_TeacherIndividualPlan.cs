namespace Models.Models.Tables;

public class StudyGroupTeacherIndividualPlan(int id) : TableBase(id)
{
    public int IdStudyGroup { get; set; }
    public int IdTeacherIndividualPlan { get; set; }
}