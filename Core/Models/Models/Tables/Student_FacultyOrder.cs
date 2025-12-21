namespace Models.Models.Tables;

public class StudentFacultyOrder(int id) : TableBase(id)
{
    public int IdStudent { get; set; }
    public int IdFacultyOrder { get; set; }
}