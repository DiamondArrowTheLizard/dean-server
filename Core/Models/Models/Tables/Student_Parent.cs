namespace Models.Models.Tables;

public class Student_Parent(int id) : TableBase(id)
{
    public int IdStudent { get; set; }
    public int IdParent { get; set; }
}