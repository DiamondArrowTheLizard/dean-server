namespace Models.Models.Tables;

public class Student_QualificationWork(int id) : TableBase(id)
{
    public Student_QualificationWork() : this(0) { }
    public int IdStudent { get; set; }
    public int IdQualificationWork { get; set; }
}