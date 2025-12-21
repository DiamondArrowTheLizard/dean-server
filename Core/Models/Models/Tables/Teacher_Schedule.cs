namespace Models.Models.Tables;

public class TeacherSchedule(int id) : TableBase(id)
{
    public int IdTeacher { get; set; }
    public int IdSchedule { get; set; }
}