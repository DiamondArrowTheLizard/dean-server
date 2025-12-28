namespace Models.Models.Tables;

public class StudyGroup_TeacherIndividualPlan(int id) : TableBase(id)
{
    public StudyGroup_TeacherIndividualPlan() : this(0) { }
    public int IdStudyGroup { get; set; }
    public int IdTeacherIndividualPlan { get; set; }
}