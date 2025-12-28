using System.Reflection;
using Interfaces.Services;
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
            ["GetAllStudyGroups"] = @"SELECT 
    sg.id,
    sg.group_number AS GroupNumber,
    sg.id_course AS IdCourse,
    c.course_name AS CourseName
FROM StudyGroup sg
LEFT JOIN Course c ON sg.id_course = c.id
ORDER BY sg.id;",

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
            ["GetAllSchedules"] = @"SELECT 
    s.id,
    s.study_weeks AS StudyWeeks,
    s.day_of_week AS DayOfWeekEnum,
    s.start_time AS StartTime,
    s.end_time AS EndTime,
    s.class_type AS ClassTypeEnum,
    s.id_studygroup AS IdStudyGroup,
    s.id_discipline AS IdDiscipline,
    sg.group_number AS GroupNumber,
    d.discipline_name AS DisciplineName
FROM Schedule s
LEFT JOIN StudyGroup sg ON s.id_studygroup = sg.id
LEFT JOIN Discipline d ON s.id_discipline = d.id
ORDER BY s.id;",

            ["GetAllDisciplines"] = @"SELECT id, discipline_name AS DisciplineName FROM Discipline ORDER BY discipline_name;",

            ["InsertSchedule"] = @"INSERT INTO Schedule (
    study_weeks, day_of_week, start_time, end_time, class_type, id_studygroup, id_discipline
) VALUES (
    @studyWeeks, @dayOfWeek::day_of_week_enum, @startTime, @endTime, @classType::class_type_enum, @idStudyGroup, @idDiscipline
) RETURNING id;",

            ["UpdateSchedule"] = @"UPDATE Schedule SET 
    study_weeks = @studyWeeks,
    day_of_week = @dayOfWeek::day_of_week_enum,
    start_time = @startTime,
    end_time = @endTime,
    class_type = @classType::class_type_enum,
    id_studygroup = @idStudyGroup,
    id_discipline = @idDiscipline
WHERE id = @id;",

            ["DeleteSchedule"] = @"DELETE FROM Schedule WHERE id = @id;",

            ["DeleteScheduleWithDependencies"] = @"DELETE FROM Teacher_Schedule WHERE id_schedule = @id;
DELETE FROM Classroom_Schedule WHERE id_schedule = @id;
DELETE FROM Schedule WHERE id = @id;",
            ["GetAllPerformances"] = @"SELECT 
    p.id,
    p.mark_type AS MarkTypeEnum,
    p.mark AS Mark,
    p.id_teacher AS IdTeacher,
    p.id_student AS IdStudent,
    t.last_name || ' ' || t.first_name || ' ' || t.middle_name AS TeacherName,
    s.last_name || ' ' || s.first_name || ' ' || s.middle_name AS StudentName,
    sg.group_number AS GroupNumber
FROM Performance p
LEFT JOIN Teacher t ON p.id_teacher = t.id
LEFT JOIN Student s ON p.id_student = s.id
LEFT JOIN StudyGroup sg ON s.id_studygroup = sg.id
ORDER BY p.id;",

            ["GetAllTeachers"] = @"SELECT 
    id,
    last_name AS LastName,
    first_name AS FirstName,
    middle_name AS MiddleName,
    id_position AS IdPosition,
    id_department AS IdDepartment
FROM Teacher ORDER BY last_name, first_name;",

            ["GetAllKnowledgeCheckTypes"] = @"SELECT id, knowledge_check_type AS KnowledgeCheckTypeEnum FROM KnowledgeCheckType ORDER BY id;",

            ["InsertPerformance"] = @"INSERT INTO Performance (
    mark_type, mark, id_teacher, id_student
) VALUES (
    @markType::mark_type_enum, @mark, @idTeacher, @idStudent
) RETURNING id;",

            ["UpdatePerformance"] = @"UPDATE Performance SET 
    mark_type = @markType::mark_type_enum,
    mark = @mark,
    id_teacher = @idTeacher,
    id_student = @idStudent
WHERE id = @id;",

            ["DeletePerformance"] = @"DELETE FROM Performance WHERE id = @id;",

            ["DeletePerformanceWithDependencies"] = @"DELETE FROM Curriculum_Performance WHERE id_performance = @id;
DELETE FROM Performance_KnowledgeCheckType WHERE id_performance = @id;
DELETE FROM Performance_Discipline WHERE id_performance = @id;
DELETE FROM Performance WHERE id = @id;",
            ["GetAllQualificationWorks"] = @"SELECT 
    qw.id,
    qw.work_name AS WorkName,
    qw.id_teacher AS IdTeacher,
    t.last_name || ' ' || t.first_name || ' ' || t.middle_name AS TeacherName
FROM QualificationWork qw
LEFT JOIN Teacher t ON qw.id_teacher = t.id
ORDER BY qw.id;",

            ["InsertQualificationWork"] = @"INSERT INTO QualificationWork (
    work_name, id_teacher
) VALUES (
    @workName, @idTeacher
) RETURNING id;",

            ["UpdateQualificationWork"] = @"UPDATE QualificationWork SET 
    work_name = @workName,
    id_teacher = @idTeacher
WHERE id = @id;",

            ["DeleteQualificationWork"] = @"DELETE FROM QualificationWork WHERE id = @id;",

            ["DeleteQualificationWorkWithDependencies"] = @"DELETE FROM Student_QualificationWork WHERE id_qualificationwork = @id;
DELETE FROM QualificationWork WHERE id = @id;",
            ["GetAllCurriculums"] = @"SELECT 
    c.id,
    c.total_hours AS TotalHours,
    c.lecture_hours AS LectureHours,
    c.practice_hours AS PracticeHours,
    c.lab_hours AS LabHours,
    c.id_discipline AS IdDiscipline,
    c.id_course AS IdCourse,
    c.id_studygroup AS IdStudyGroup,
    c.id_knowledgechecktype AS IdKnowledgeCheckType,
    d.discipline_name AS DisciplineName,
    cr.course_name AS CourseName,
    sg.group_number AS GroupNumber,
    kct.knowledge_check_type AS KnowledgeCheckTypeEnum
FROM Curriculum c
LEFT JOIN Discipline d ON c.id_discipline = d.id
LEFT JOIN Course cr ON c.id_course = cr.id
LEFT JOIN StudyGroup sg ON c.id_studygroup = sg.id
LEFT JOIN KnowledgeCheckType kct ON c.id_knowledgechecktype = kct.id
ORDER BY c.id;",

            ["GetAllCourses"] = @"SELECT id, course_name AS CourseName FROM Course ORDER BY course_name;",

            ["InsertCurriculum"] = @"INSERT INTO Curriculum (
    total_hours, lecture_hours, practice_hours, lab_hours, id_discipline, id_course, id_studygroup, id_knowledgechecktype
) VALUES (
    @totalHours, @lectureHours, @practiceHours, @labHours, @idDiscipline, @idCourse, @idStudyGroup, @idKnowledgeCheckType
) RETURNING id;",

            ["UpdateCurriculum"] = @"UPDATE Curriculum SET 
    total_hours = @totalHours,
    lecture_hours = @lectureHours,
    practice_hours = @practiceHours,
    lab_hours = @labHours,
    id_discipline = @idDiscipline,
    id_course = @idCourse,
    id_studygroup = @idStudyGroup,
    id_knowledgechecktype = @idKnowledgeCheckType
WHERE id = @id;",

            ["DeleteCurriculum"] = @"DELETE FROM Curriculum WHERE id = @id;",

            ["DeleteCurriculumWithDependencies"] = @"DELETE FROM Curriculum_Performance WHERE id_curriculum = @id;
DELETE FROM Curriculum WHERE id = @id;",
            ["InsertStudyGroup"] = @"INSERT INTO StudyGroup (
    group_number, id_course
) VALUES (
    @groupNumber, @idCourse
) RETURNING id;",

            ["UpdateStudyGroup"] = @"UPDATE StudyGroup SET 
    group_number = @groupNumber,
    id_course = @idCourse
WHERE id = @id;",

            ["DeleteStudyGroup"] = @"DELETE FROM StudyGroup WHERE id = @id;",

            ["DeleteStudyGroupWithDependencies"] = @"DELETE FROM Schedule WHERE id_studygroup = @id;
DELETE FROM Student WHERE id_studygroup = @id;
DELETE FROM StudyGroup_TeacherIndividualPlan WHERE id_studygroup = @id;
DELETE FROM Curriculum WHERE id_studygroup = @id;
DELETE FROM StudyGroup WHERE id = @id;",
            ["GetAllTeacherIndividualPlans"] = @"SELECT 
    tip.id,
    tip.total_hours AS TotalHours,
    tip.lecture_hours AS LectureHours,
    tip.practice_hours AS PracticeHours,
    tip.lab_hours AS LabHours,
    tip.id_teacher AS IdTeacher,
    t.last_name || ' ' || t.first_name || ' ' || t.middle_name AS TeacherName
FROM TeacherIndividualPlan tip
LEFT JOIN Teacher t ON tip.id_teacher = t.id
ORDER BY tip.id;",

            ["InsertTeacherIndividualPlan"] = @"INSERT INTO TeacherIndividualPlan (
    total_hours, lecture_hours, practice_hours, lab_hours, id_teacher
) VALUES (
    @totalHours, @lectureHours, @practiceHours, @labHours, @idTeacher
) RETURNING id;",

            ["UpdateTeacherIndividualPlan"] = @"UPDATE TeacherIndividualPlan SET 
    total_hours = @totalHours,
    lecture_hours = @lectureHours,
    practice_hours = @practiceHours,
    lab_hours = @labHours,
    id_teacher = @idTeacher
WHERE id = @id;",

            ["DeleteTeacherIndividualPlan"] = @"DELETE FROM TeacherIndividualPlan WHERE id = @id;",

            ["DeleteTeacherIndividualPlanWithDependencies"] = @"DELETE FROM StudyGroup_TeacherIndividualPlan WHERE id_teacherindividualplan = @id;
DELETE FROM TeacherIndividualPlan_KnowledgeCheckType WHERE id_teacherindividualplan = @id;
DELETE FROM TeacherIndividualPlan_Discipline WHERE id_teacherindividualplan = @id;
DELETE FROM TeacherIndividualPlan WHERE id = @id;",
        };

        return queries.ContainsKey(queryName) ? queries[queryName] : string.Empty;
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
                            else if (propType == typeof(TimeSpan))
                            {
                                if (value is TimeSpan timeSpanValue)
                                {
                                    convertedValue = timeSpanValue;
                                }
                                else if (value.GetType().Name == "TimeOnly")
                                {
                                    var timeOnlyType = value.GetType();
                                    var hourProperty = timeOnlyType.GetProperty("Hour");
                                    var minuteProperty = timeOnlyType.GetProperty("Minute");
                                    var secondProperty = timeOnlyType.GetProperty("Second");

                                    if (hourProperty != null && minuteProperty != null && secondProperty != null)
                                    {
                                        var hour = Convert.ToInt32(hourProperty.GetValue(value));
                                        var minute = Convert.ToInt32(minuteProperty.GetValue(value));
                                        var second = Convert.ToInt32(secondProperty.GetValue(value));
                                        convertedValue = new TimeSpan(hour, minute, second);
                                    }
                                    else
                                    {
                                        convertedValue = TimeSpan.Parse(value.ToString() ?? "00:00:00");
                                    }
                                }
                                else
                                {
                                    convertedValue = TimeSpan.Parse(value.ToString() ?? "00:00:00");
                                }
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
                    else if (parameterValue is TimeSpan timeSpanValue)
                    {
                        command.Parameters.AddWithValue(param.Key, timeSpanValue);
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
                    else if (parameterValue is TimeSpan timeSpanValue)
                    {
                        command.Parameters.AddWithValue(param.Key, timeSpanValue);
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

            if (typeof(T) == typeof(TimeSpan) && result.GetType().Name == "TimeOnly")
            {
                var timeOnlyType = result.GetType();
                var hourProperty = timeOnlyType.GetProperty("Hour");
                var minuteProperty = timeOnlyType.GetProperty("Minute");
                var secondProperty = timeOnlyType.GetProperty("Second");

                if (hourProperty != null && minuteProperty != null && secondProperty != null)
                {
                    var hour = Convert.ToInt32(hourProperty.GetValue(result));
                    var minute = Convert.ToInt32(minuteProperty.GetValue(result));
                    var second = Convert.ToInt32(secondProperty.GetValue(result));
                    var timeSpanValue = new TimeSpan(hour, minute, second);
                    return (T)(object)timeSpanValue;
                }
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