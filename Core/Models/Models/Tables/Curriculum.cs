namespace Models.Models.Tables;

public class Curriculum(int id) : TableBase(id)
{
    public int TotalHours { get; set; }
    public int LectureHours { get; set; }
    public int PracticeHours { get; set; }
    public int LabHours { get; set; }
    public int IdDiscipline { get; set; }
    public int IdCourse { get; set; }
    public int IdStudyGroup { get; set; }
    public int IdKnowledgeCheckType { get; set; }
}