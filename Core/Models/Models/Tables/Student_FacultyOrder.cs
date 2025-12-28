namespace Models.Models.Tables;

public class Student_FacultyOrder(int id) : TableBase(id)
{
    public Student_FacultyOrder() : this(0) { }
    public int IdStudent { get; set; }
    public int IdFacultyOrder { get; set; }
}