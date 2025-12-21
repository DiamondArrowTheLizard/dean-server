namespace Models.Models.Tables;

public class StudentQualificationWork(int id) : TableBase(id)
{
    public int IdStudent { get; set; }
    public int IdQualificationWork { get; set; }
}