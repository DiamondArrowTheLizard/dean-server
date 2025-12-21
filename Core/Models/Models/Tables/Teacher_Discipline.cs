namespace Models.Models.Tables;

public class TeacherDiscipline(int id) : TableBase(id)
{
    public int IdTeacher { get; set; }
    public int IdDiscipline { get; set; }
}