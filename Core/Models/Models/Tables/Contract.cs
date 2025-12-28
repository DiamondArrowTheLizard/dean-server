namespace Models.Models.Tables;

public class Contract(int id) : TableBase(id)
{
    public Contract() : this(0) { }
    public string ContractTypeEnum { get; set; } = string.Empty;
    public DateTime DateOfSign { get; set; }
    public DateTime DateOfExpire { get; set; }
    public int IdStudent { get; set; }
}