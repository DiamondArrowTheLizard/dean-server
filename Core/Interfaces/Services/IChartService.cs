using System.Threading.Tasks;

namespace Interfaces.Services
{
    public interface IChartData
    {
        public Dictionary<int, int> StudentsByCourse { get; set; }
        public Dictionary<string, double> PerformanceByFaculty { get; set; }
        public Dictionary<string, int> EducationBasisDistribution { get; set; }
        public Dictionary<int, int> EnrollmentByYear { get; set; }
    }
    public interface IChartService
    {
        Task<string> GenerateChartsAsync();
        Task<string> GenerateChartsPdfAsync();
        Task<IChartData> GetChartDataAsync();
    }
}