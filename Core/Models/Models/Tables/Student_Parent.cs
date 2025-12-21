namespace Models.Models.Tables;

public class StudentParent(int id) : TableBase(id)
{
    public int IdStudent { get; set; }
    public int IdParent { get; set; }
}