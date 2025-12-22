namespace Models.Models.Tables;

public class Teacher_AcademicTitle(int id) : TableBase(id)
{
    public int IdTeacher { get; set; }
    public int IdAcademicTitle { get; set; }
}