namespace Models.Models.Tables;

public class Classroom_Schedule(int id) : TableBase(id)
{
    public int IdClassroom { get; set; }
    public int IdSchedule { get; set; }
}