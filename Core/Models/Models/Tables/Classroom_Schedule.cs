namespace Models.Models.Tables;

public class Classroom_Schedule(int id) : TableBase(id)
{
    public Classroom_Schedule() : this(0) { }
    public int IdClassroom { get; set; }
    public int IdSchedule { get; set; }
}