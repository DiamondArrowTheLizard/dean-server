namespace Models.Models.Tables;

public class Payment(int id) : TableBase(id)
{
    public float PaymentSum { get; set; }
    public DateTime DateOfExpire { get; set; }
    public int IdContract { get; set; }
}