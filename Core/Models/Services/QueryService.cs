using System.Reflection;
using Core.Interfaces.Services;
using Interfaces.Models;
using Npgsql;

namespace Models.Services;

public class QueryService(IDatabaseConnectionString connectionString) : IQueryService
{
    private readonly IDatabaseConnectionString _connectionString = connectionString;

    public string GetQuery(string queryName)
    {
        var queries = new Dictionary<string, string>
        {
            ["GetAllDepartments"] = @"SELECT 
                d.id,
                d.department_name AS DepartmentName,
                f.faculty_name AS FacultyName,
                d.id_faculty AS IdFaculty
            FROM Department d
            JOIN Faculty f ON d.id_faculty = f.id
            ORDER BY d.id;",

            ["GetAllFaculties"] = @"SELECT id, faculty_name AS FacultyName FROM Faculty ORDER BY faculty_name;",

            ["InsertDepartment"] = @"INSERT INTO Department (department_name, id_faculty) 
                VALUES (@departmentName, @idFaculty) RETURNING id;",

            ["UpdateDepartment"] = @"UPDATE Department 
                SET department_name = @departmentName, id_faculty = @idFaculty 
                WHERE id = @id;",

            ["DeleteDepartment"] = @"DELETE FROM Department WHERE id = @id;",

            ["GetAllClassrooms"] = @"SELECT id, classroom_name AS ClassroomName FROM Classroom ORDER BY id;",

            ["InsertClassroom"] = @"INSERT INTO Classroom (classroom_name) 
                VALUES (@classroomName) RETURNING id;",

            ["UpdateClassroom"] = @"UPDATE Classroom 
                SET classroom_name = @classroomName 
                WHERE id = @id;",

            ["DeleteClassroom"] = @"DELETE FROM Classroom WHERE id = @id;",
            
            ["GetAllStudents"] = @"SELECT 
    s.id,
    s.last_name AS LastName,
    s.first_name AS FirstName,
    s.middle_name AS MiddleName,
    s.gender AS GenderEnum,
    s.birth_date AS BirthDate,
    s.has_children AS HasChildren,
    s.phone_number AS PhoneNumber,
    s.email AS Email,
    s.snils AS Snils,
    s.student_status AS StudentStatusEnum,
    s.education_basis AS EducationBasisEnum,
    s.enrollment_year AS EnrollmentYear,
    s.course AS Course,
    s.scholarship_amount AS ScholarshipAmount,
    s.house_number AS HouseNumber,
    s.id_studygroup AS IdStudyGroup,
    s.id_street AS IdStreet,
    s.id_city AS IdCity,
    c.city_name AS CityName,
    st.street_name AS StreetName,
    sg.group_number AS GroupNumber,
    cr.course_name AS CourseName,
    f.faculty_name AS FacultyName
FROM Student s
LEFT JOIN City c ON s.id_city = c.id
LEFT JOIN Street st ON s.id_street = st.id
LEFT JOIN StudyGroup sg ON s.id_studygroup = sg.id
LEFT JOIN Course cr ON sg.id_course = cr.id
LEFT JOIN Faculty f ON cr.id_faculty = f.id
ORDER BY s.id;",

            ["GetAllCities"] = @"SELECT id, city_name AS CityName FROM City ORDER BY city_name;",
            ["GetAllStreets"] = @"SELECT id, street_name AS StreetName FROM Street ORDER BY street_name;",
            ["GetAllStudyGroups"] = @"SELECT sg.id, sg.group_number AS GroupNumber FROM StudyGroup sg ORDER BY sg.group_number;",

            ["InsertStudent"] = @"INSERT INTO Student (
    last_name, first_name, middle_name, gender, birth_date, has_children,
    phone_number, email, snils, student_status, education_basis, enrollment_year,
    course, scholarship_amount, house_number, id_studygroup, id_street, id_city
) VALUES (
    @lastName, @firstName, @middleName, @gender::gender_enum, @birthDate, @hasChildren,
    @phoneNumber, @email, @snils, @studentStatus::student_status_enum, @educationBasis::education_basis_enum, @enrollmentYear,
    @course, @scholarshipAmount, @houseNumber, @idStudyGroup, @idStreet, @idCity
) RETURNING id;",

            ["UpdateStudent"] = @"UPDATE Student SET 
    last_name = @lastName,
    first_name = @firstName,
    middle_name = @middleName,
    gender = @gender::gender_enum,
    birth_date = @birthDate,
    has_children = @hasChildren,
    phone_number = @phoneNumber,
    email = @email,
    snils = @snils,
    student_status = @studentStatus::student_status_enum,
    education_basis = @educationBasis::education_basis_enum,
    enrollment_year = @enrollmentYear,
    course = @course,
    scholarship_amount = @scholarshipAmount,
    house_number = @houseNumber,
    id_studygroup = @idStudyGroup,
    id_street = @idStreet,
    id_city = @idCity
WHERE id = @id;",

            ["DeleteStudent"] = @"DELETE FROM Student WHERE id = @id;",

            ["DeleteStudentWithDependencies"] = @"DELETE FROM Curriculum_Performance WHERE id_performance IN (SELECT id FROM Performance WHERE id_student = @id);
DELETE FROM Performance_KnowledgeCheckType WHERE id_performance IN (SELECT id FROM Performance WHERE id_student = @id);
DELETE FROM Performance_Discipline WHERE id_performance IN (SELECT id FROM Performance WHERE id_student = @id);
DELETE FROM Payment WHERE id_contract IN (SELECT id FROM Contract WHERE id_student = @id);
DELETE FROM Contract WHERE id_student = @id;
DELETE FROM Student_QualificationWork WHERE id_student = @id;
DELETE FROM Student_Parent WHERE id_student = @id;
DELETE FROM Student_FacultyOrder WHERE id_student = @id;
DELETE FROM Performance WHERE id_student = @id;
DELETE FROM Student WHERE id = @id;",

            ["DeleteStudentQualifications"] = @"DELETE FROM Student_QualificationWork WHERE id_student = @id;",
            ["DeleteStudentPerformance"] = @"DELETE FROM Performance WHERE id_student = @id;",
            ["DeleteStudentContracts"] = @"DELETE FROM Contract WHERE id_student = @id;",
            ["DeleteStudentPayments"] = @"DELETE FROM Payment WHERE id_contract IN (SELECT id FROM Contract WHERE id_student = @id);",
            ["DeleteStudentParents"] = @"DELETE FROM Student_Parent WHERE id_student = @id;",
            ["DeleteStudentFacultyOrders"] = @"DELETE FROM Student_FacultyOrder WHERE id_student = @id;",
            ["DeletePerformanceRelated"] = @"DELETE FROM Curriculum_Performance WHERE id_performance IN (SELECT id FROM Performance WHERE id_student = @id);
DELETE FROM Performance_KnowledgeCheckType WHERE id_performance IN (SELECT id FROM Performance WHERE id_student = @id);
DELETE FROM Performance_Discipline WHERE id_performance IN (SELECT id FROM Performance WHERE id_student = @id);",
            ["DeleteDepartmentWithDependencies"] = @"DELETE FROM Teacher WHERE id_department = @id;
DELETE FROM Department WHERE id = @id;",

            ["DeleteClassroomWithDependencies"] = @"DELETE FROM Classroom_Schedule WHERE id_classroom = @id;
DELETE FROM Classroom WHERE id = @id;",
        };

        return queries.ContainsKey(queryName) ? queries[queryName] : string.Empty;
    }

    public Dictionary<string, string> GetAvailableQueries()
    {
        return new Dictionary<string, string>
        {
            ["GetAllDepartments"] = "Получить все кафедры",
            ["GetAllFaculties"] = "Получить все факультеты",
            ["InsertDepartment"] = "Добавить кафедру",
            ["UpdateDepartment"] = "Обновить кафедру",
            ["DeleteDepartment"] = "Удалить кафедру",
            ["GetAllClassrooms"] = "Получить все аудитории",
            ["InsertClassroom"] = "Добавить аудиторию",
            ["UpdateClassroom"] = "Обновить аудиторию",
            ["DeleteClassroom"] = "Удалить аудиторию",
            ["GetAllStudents"] = "Получить всех студентов",
            ["GetAllCities"] = "Получить все города",
            ["GetAllStreets"] = "Получить все улицы",
            ["GetAllStudyGroups"] = "Получить все группы",
            ["InsertStudent"] = "Добавить студента",
            ["UpdateStudent"] = "Обновить студента",
            ["DeleteStudent"] = "Удалить студента",
            ["DeleteStudentWithDependencies"] = "Удалить студента со всеми зависимостями",
            ["DeleteStudentQualifications"] = "Удалить квалификационные работы студента",
            ["DeleteStudentPerformance"] = "Удалить успеваемость студента",
            ["DeleteStudentContracts"] = "Удалить договоры студента",
            ["DeleteStudentPayments"] = "Удалить платежи студента",
            ["DeleteStudentParents"] = "Удалить связи с родителями студента",
            ["DeleteStudentFacultyOrders"] = "Удалить приказы студента",
            ["DeletePerformanceRelated"] = "Удалить связанные записи успеваемости",
            ["DeleteDepartmentWithDependencies"] = "Удалить Кафедру с зависимостями",
            ["DeleteClassroomWithDependencies"] = "Удалить Аудиторию с зависимостями",
        };
    }

    private void CheckConnection()
    {
        if (string.IsNullOrEmpty(_connectionString.ConnectionString))
        {
            throw new InvalidOperationException("Строка подключения не установлена. Выполните аутентификацию.");
        }
    }

    public async Task<IEnumerable<T>> ExecuteQueryAsync<T>(string query) where T : class, new()
    {
        CheckConnection();

        var results = new List<T>();
        var connectionString = _connectionString.ConnectionString;

        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var item = new T();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var propName = reader.GetName(i);
                    var prop = typeof(T).GetProperty(propName,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (prop != null && !reader.IsDBNull(i))
                    {
                        var value = reader.GetValue(i);
                        var propType = prop.PropertyType;

                        try
                        {
                            object convertedValue;

                            if (value == null || value is DBNull)
                            {
                                continue;
                            }

                            if (propType == typeof(string))
                            {
                                var stringValue = value.ToString() ?? throw new ArgumentNullException(nameof(value), $"Значение для свойства {propName} не может быть null");
                                convertedValue = stringValue;
                            }
                            else if (propType == typeof(int))
                            {
                                convertedValue = Convert.ToInt32(value);
                            }
                            else if (propType == typeof(bool))
                            {
                                convertedValue = Convert.ToBoolean(value);
                            }
                            else if (propType == typeof(DateTime))
                            {
                                if (value is DateTime dateTimeValue)
                                {
                                    convertedValue = dateTimeValue;
                                }
                                else if (value.GetType().Name == "DateOnly")
                                {
                                    convertedValue = ConvertDateOnlyToDateTime(value);
                                }
                                else
                                {
                                    convertedValue = Convert.ToDateTime(value);
                                }
                            }
                            else if (propType == typeof(decimal))
                            {
                                convertedValue = Convert.ToDecimal(value);
                            }
                            else if (propType == typeof(double))
                            {
                                convertedValue = Convert.ToDouble(value);
                            }
                            else if (propType == typeof(float))
                            {
                                convertedValue = Convert.ToSingle(value);
                            }
                            else if (propType.IsEnum)
                            {
                                var enumString = value.ToString() ?? throw new ArgumentNullException(nameof(value), $"Значение для перечисления {propName} не может быть null");
                                convertedValue = Enum.Parse(propType, enumString, true);
                            }
                            else
                            {
                                convertedValue = value;
                            }

                            prop.SetValue(item, convertedValue);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка преобразования значения {value} ({value.GetType()}) для свойства {propName} типа {propType}: {ex.Message}");
                            throw;
                        }
                    }
                }
                results.Add(item);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка выполнения запроса: {ex.Message}");
            throw;
        }

        return results;
    }

    private DateTime ConvertDateOnlyToDateTime(object dateOnlyValue)
    {
        var dateOnlyType = dateOnlyValue.GetType();
        var yearProperty = dateOnlyType.GetProperty("Year");
        var monthProperty = dateOnlyType.GetProperty("Month");
        var dayProperty = dateOnlyType.GetProperty("Day");

        if (yearProperty == null || monthProperty == null || dayProperty == null)
        {
            throw new InvalidOperationException($"Не удалось найти свойства Year, Month, Day в типе {dateOnlyType}");
        }

        
        var year = yearProperty.GetValue(dateOnlyValue) is int yearValue 
            ? yearValue 
            : throw new InvalidOperationException("Year property returned null");
        
        var month = monthProperty.GetValue(dateOnlyValue) is int monthValue 
            ? monthValue 
            : throw new InvalidOperationException("Month property returned null");
            
        var day = dayProperty.GetValue(dateOnlyValue) is int dayValue 
            ? dayValue 
            : throw new InvalidOperationException("Day property returned null");

        return new DateTime(year, month, day);
    }

    public async Task<int> ExecuteNonQueryAsync(string query, Dictionary<string, object>? parameters = null)
    {
        CheckConnection();

        var connectionString = _connectionString.ConnectionString;

        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    var parameterValue = param.Value ?? DBNull.Value;
                    
                    if (parameterValue is DateTime dateTimeValue)
                    {
                        command.Parameters.AddWithValue(param.Key, dateTimeValue);
                    }
                    else if (parameterValue is Enum)
                    {
                        
                        var stringValue = parameterValue.ToString();
                        command.Parameters.AddWithValue(param.Key, stringValue?.ToLower() ?? string.Empty);
                    }
                    else
                    {
                        command.Parameters.AddWithValue(param.Key, parameterValue);
                    }
                }
            }

            return await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка выполнения команды: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<Dictionary<string, object?>>> ExecuteQueryRawAsync(string query)
    {
        CheckConnection();

        var results = new List<Dictionary<string, object?>>();
        var connectionString = _connectionString.ConnectionString;

        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object?>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);
                    var value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    row[columnName] = value;
                }
                results.Add(row);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка выполнения запроса: {ex.Message}");
            throw;
        }

        return results;
    }

    public async Task<T?> ExecuteScalarAsync<T>(string query, Dictionary<string, object>? parameters = null)
    {
        CheckConnection();

        var connectionString = _connectionString.ConnectionString;

        try
        {
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            await using var command = new NpgsqlCommand(query, connection);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    var parameterValue = param.Value ?? DBNull.Value;
                    
                    if (parameterValue is DateTime dateTimeValue)
                    {
                        command.Parameters.AddWithValue(param.Key, dateTimeValue);
                    }
                    else if (parameterValue is Enum)
                    {
                        
                        var stringValue = parameterValue.ToString();
                        command.Parameters.AddWithValue(param.Key, stringValue?.ToLower() ?? string.Empty);
                    }
                    else
                    {
                        command.Parameters.AddWithValue(param.Key, parameterValue);
                    }
                }
            }

            var result = await command.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value)
            {
                return default;
            }

            if (typeof(T) == typeof(DateTime) && result.GetType().Name == "DateOnly")
            {
                var dateTimeValue = ConvertDateOnlyToDateTime(result);
                return (T)(object)dateTimeValue;
            }

            return (T)Convert.ChangeType(result, typeof(T));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка выполнения скалярного запроса: {ex.Message}");
            throw;
        }
    }
}