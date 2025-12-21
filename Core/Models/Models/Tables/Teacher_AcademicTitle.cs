namespace Models.Models.Tables;

public class TeacherAcademicTitle(int id) : TableBase(id)
{
    public int IdTeacher { get; set; }
    public int IdAcademicTitle { get; set; }
}