namespace Models.Models.Tables;

public class Student(int id) : TableBase(id)
{
    public Student() : this(0) { }
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string GenderEnum { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public bool HasChildren { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Snils { get; set; } = string.Empty;
    public string StudentStatusEnum { get; set; } = string.Empty;
    public string EducationBasisEnum { get; set; } = string.Empty;
    public DateTime EnrollmentYear { get; set; }
    public int Course { get; set; }
    public int ScholarshipAmount { get; set; }
    public string HouseNumber { get; set; } = string.Empty;
    public int IdStudyGroup { get; set; }
    public int IdStreet { get; set; }
    public int IdCity { get; set; }
}