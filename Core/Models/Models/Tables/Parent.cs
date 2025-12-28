namespace Models.Models.Tables;

public class Parent(int id) : TableBase(id)
{
    public Parent() : this(0) { }
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RelationshipDegreeEnum { get; set; } = string.Empty;
}