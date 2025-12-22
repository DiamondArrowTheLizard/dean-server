namespace Models.Models.Tables;

public class Student_QualificationWork(int id) : TableBase(id)
{
    public int IdStudent { get; set; }
    public int IdQualificationWork { get; set; }
}