namespace Models.Models.Tables;

public class Teacher_Discipline(int id) : TableBase(id)
{
    public Teacher_Discipline() : this(0) { }
    public int IdTeacher { get; set; }
    public int IdDiscipline { get; set; }
}