namespace Models.Models.Tables;

public class Teacher_AcademicDegree(int id) : TableBase(id)
{
    public Teacher_AcademicDegree() : this(0) { }
    public int IdTeacher { get; set; }
    public int IdAcademicDegree { get; set; }
}