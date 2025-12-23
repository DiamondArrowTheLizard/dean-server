namespace Models.Models.Tables;

public class City(int id) : TableBase(id)
{
    public string CityName { get; set; } = string.Empty;
}