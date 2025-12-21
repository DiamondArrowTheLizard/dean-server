namespace Models.Models.Tables;

public class TeacherAcademicDegree(int id) : TableBase(id)
{
    public int IdTeacher { get; set; }
    public int IdAcademicDegree { get; set; }
}