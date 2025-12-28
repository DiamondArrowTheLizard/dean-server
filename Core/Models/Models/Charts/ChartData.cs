using Interfaces.Services;

namespace Core.Models.Charts
{
    public class ChartData : IChartData
    {
        public Dictionary<int, int> StudentsByCourse { get; set; } = [];
        public Dictionary<string, double> PerformanceByFaculty { get; set; } = [];
        public Dictionary<string, int> EducationBasisDistribution { get; set; } = [];
        public Dictionary<int, int> EnrollmentByYear { get; set; } = [];
    }
}