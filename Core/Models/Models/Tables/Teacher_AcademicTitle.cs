namespace Models.Models.Tables;

public class Teacher_AcademicTitle(int id) : TableBase(id)
{
    public Teacher_AcademicTitle() : this(0) { }
    public int IdTeacher { get; set; }
    public int IdAcademicTitle { get; set; }
}