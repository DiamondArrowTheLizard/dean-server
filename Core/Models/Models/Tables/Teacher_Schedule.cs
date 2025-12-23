namespace Models.Models.Tables;

public class Teacher_Schedule(int id) : TableBase(id)
{
    public int IdTeacher { get; set; }
    public int IdSchedule { get; set; }
}