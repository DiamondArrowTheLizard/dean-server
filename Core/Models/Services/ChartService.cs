using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Interfaces.Services;
using Core.Models.Charts;
using Npgsql;

namespace Models.Services
{
    public class ChartService : IChartService
    {
        private readonly string _connectionString = string.Empty;
        private readonly string _pythonScriptPath = string.Empty;

        public ChartService(string connectionString)
        {
            _connectionString = connectionString;
            _pythonScriptPath = $"{FindProjectRoot()}/Python/charts_generator.py";
        }

        private string FindProjectRoot()
        {
            var currentDir = Directory.GetCurrentDirectory();
            var directory = new DirectoryInfo(currentDir);

            while (directory != null)
            {
                var pythonDir = Path.Combine(directory.FullName, "Python");
                var pythonFile = Path.Combine(pythonDir, "charts_generator.py");

                if (Directory.Exists(pythonDir) && File.Exists(pythonFile))
                {
                    return directory.FullName;
                }

                var solutionFile = Directory.GetFiles(directory.FullName, "*.sln").FirstOrDefault();
                if (solutionFile != null)
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            throw new DirectoryNotFoundException("Project root not found. Make sure Python folder exists at project root.");
        }

        public async Task<IChartData> GetChartDataAsync()
        {
            var data = new ChartData();

            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            await LoadStudentsByCourse(data, connection);
            await LoadPerformanceByFaculty(data, connection);
            await LoadEducationBasisDistribution(data, connection);
            await LoadEnrollmentByYear(data, connection);

            return data;
        }

        private async Task LoadStudentsByCourse(ChartData data, NpgsqlConnection connection)
        {
            var query = @"
                SELECT course, COUNT(*) as count 
                FROM Student 
                WHERE student_status IN ('active', 'graduated')
                GROUP BY course 
                ORDER BY course";

            using var command = new NpgsqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                data.StudentsByCourse[reader.GetInt32(0)] = reader.GetInt32(1);
            }
        }

        private async Task LoadPerformanceByFaculty(ChartData data, NpgsqlConnection connection)
        {
            var query = @"
                SELECT f.faculty_name, AVG(p.mark) as avg_mark
                FROM Faculty f
                JOIN Department d ON f.id = d.id_faculty
                JOIN Teacher t ON d.id = t.id_department
                JOIN Performance p ON t.id = p.id_teacher
                WHERE p.mark_type = 'mark'
                GROUP BY f.id, f.faculty_name";

            using var command = new NpgsqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                data.PerformanceByFaculty[reader.GetString(0)] = reader.GetDouble(1);
            }
        }

        private async Task LoadEducationBasisDistribution(ChartData data, NpgsqlConnection connection)
        {
            var query = @"
                SELECT education_basis::text, COUNT(*) as count
                FROM Student 
                WHERE student_status IN ('active', 'graduated')
                GROUP BY education_basis";

            using var command = new NpgsqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                data.EducationBasisDistribution[reader.GetString(0)] = reader.GetInt32(1);
            }
        }

        private async Task LoadEnrollmentByYear(ChartData data, NpgsqlConnection connection)
        {
            var query = @"
                SELECT EXTRACT(YEAR FROM enrollment_year)::int as year, COUNT(*) as count
                FROM Student
                GROUP BY EXTRACT(YEAR FROM enrollment_year)
                ORDER BY year";

            using var command = new NpgsqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                data.EnrollmentByYear[reader.GetInt32(0)] = reader.GetInt32(1);
            }
        }

        public async Task<string> GenerateChartsAsync()
        {
            try
            {
                var data = await GetChartDataAsync();
                var jsonData = JsonSerializer.Serialize(data);

                var tempDir = Path.GetTempPath();
                var outputPng = Path.Combine(tempDir, $"charts_{Guid.NewGuid()}.png");
                var outputPdf = Path.Combine(tempDir, $"charts_{Guid.NewGuid()}.pdf");

                var result = await RunPythonScriptAsync(jsonData, outputPng, outputPdf);

                if (result.Success)
                {
                    return result.PngFilePath;
                }

                throw new Exception($"Failed to generate charts: {result.ErrorMessage}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating charts: {ex.Message}", ex);
            }
        }

        public async Task<string> GenerateChartsPdfAsync()
        {
            try
            {
                var data = await GetChartDataAsync();
                var jsonData = JsonSerializer.Serialize(data);

                var tempDir = Path.GetTempPath();
                var outputPng = Path.Combine(tempDir, $"charts_{Guid.NewGuid()}.png");
                var outputPdf = Path.Combine(tempDir, $"charts_{Guid.NewGuid()}.pdf");

                var result = await RunPythonScriptAsync(jsonData, outputPng, outputPdf);

                if (result.Success)
                {
                    return result.PdfFilePath;
                }

                throw new Exception($"Failed to generate PDF: {result.ErrorMessage}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating PDF: {ex.Message}", ex);
            }
        }

        private async Task<ChartResponse> RunPythonScriptAsync(string jsonData, string outputPng, string outputPdf)
        {
            if (!File.Exists(_pythonScriptPath))
            {
                throw new FileNotFoundException($"Python script not found: {_pythonScriptPath}");
            }

            var pythonExe = FindPythonExecutable();
            if (string.IsNullOrEmpty(pythonExe))
            {
                throw new Exception("Python executable not found. Please install Python 3.7 or later.");
            }

            var arguments = $"\"{_pythonScriptPath}\" \"{outputPng}\" \"{outputPdf}\"";

            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = pythonExe,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardInputEncoding = new System.Text.UTF8Encoding(false),
                StandardOutputEncoding = System.Text.Encoding.UTF8
            };

            using var process = new System.Diagnostics.Process { StartInfo = startInfo };
            process.Start();

            await process.StandardInput.WriteAsync(jsonData);
            process.StandardInput.Close();

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
            {
                return new ChartResponse
                {
                    Success = true,
                    PngFilePath = outputPng,
                    PdfFilePath = outputPdf
                };
            }
            else
            {
                return new ChartResponse
                {
                    Success = false,
                    ErrorMessage = error
                };
            }
        }

        private string FindPythonExecutable()
        {
            var possiblePaths = new[]
            {
                "python",
                "python3",
                "python.exe",
                "python3.exe",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Python39", "python.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Python38", "python.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Python37", "python.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Python39", "python.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Python38", "python.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Python37", "python.exe")
            };

            foreach (var path in possiblePaths)
            {
                try
                {
                    var startInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = path,
                        Arguments = "--version",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    };

                    using var process = System.Diagnostics.Process.Start(startInfo);
                    if (process != null)
                    {
                        process.WaitForExit();
                        if (process.ExitCode == 0)
                        {
                            return path;
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }

            throw new ArgumentNullException("No Python executable found");
        }
    }
}