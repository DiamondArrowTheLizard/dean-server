namespace Models.Models.Tables;

public class Teacher_Schedule(int id) : TableBase(id)
{
    public Teacher_Schedule() : this(0) { }
    public int IdTeacher { get; set; }
    public int IdSchedule { get; set; }
}