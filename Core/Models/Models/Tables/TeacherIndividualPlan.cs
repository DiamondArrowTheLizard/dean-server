namespace Models.Models.Tables;

public class TeacherIndividualPlan(int id) : TableBase(id)
{
    public int TotalHours { get; set; }
    public int LectureHours { get; set; }
    public int PracticeHours { get; set; }
    public int LabHours { get; set; }
    public int IdTeacher { get; set; }
}