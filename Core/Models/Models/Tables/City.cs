namespace Models.Models.Tables;

public class City(int id) : TableBase(id)
{
    public City() : this(0) { }
    public string CityName { get; set; } = string.Empty;
}