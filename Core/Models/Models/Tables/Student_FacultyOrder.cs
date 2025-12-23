namespace Models.Models.Tables;

public class Student_FacultyOrder(int id) : TableBase(id)
{
    public int IdStudent { get; set; }
    public int IdFacultyOrder { get; set; }
}