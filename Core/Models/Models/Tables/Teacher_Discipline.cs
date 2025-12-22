namespace Models.Models.Tables;

public class Teacher_Discipline(int id) : TableBase(id)
{
    public int IdTeacher { get; set; }
    public int IdDiscipline { get; set; }
}