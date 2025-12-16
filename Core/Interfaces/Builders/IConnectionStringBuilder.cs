
using Interfaces.Models;

namespace Interfaces.Builders;

public interface IConnectionStringBuilder 
{
    public void AddHost();
    public void AddUsername();
    public void AddPassword();
    public void AddDatabase();

    public IDatabaseConnectionString GetResult();
}