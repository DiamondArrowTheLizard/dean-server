namespace Models.Models.Tables;

public class Teacher_AcademicDegree(int id) : TableBase(id)
{
    public int IdTeacher { get; set; }
    public int IdAcademicDegree { get; set; }
}