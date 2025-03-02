using Domain.Entities.BankClients;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories;

public class CompanyEmployeeRepository(IOptions<PostgresOptions> options) : ICompanyEmployeeRepository
{
    private readonly string _connectionString = options.Value.GetConnectionString();

    public async Task<int> AddAsync(CompanyEmployeeDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                INSERT INTO company_employee_records
                                (salaryprojectid, firstname, lastname, email, passportseries, passportnumber, identificationnumber, phonenumber, position, salary, isapproved) 
                                VALUES
                                (@salaryprojectid, @firstname, @lastname, @email, @passportseries, @passportnumber, @identificationnumber, @phonenumber, @position, @salary, @isapproved)
                                RETURNING id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@salaryprojectid", entity.SalaryProjectId);
        command.Parameters.AddWithValue("@firstname", entity.FirstName);
        command.Parameters.AddWithValue("@lastname", entity.LastName);
        command.Parameters.AddWithValue("@email", entity.Email);
        command.Parameters.AddWithValue("@passportseries", entity.PassportSeries);
        command.Parameters.AddWithValue("@passportnumber", entity.PassportNumber);
        command.Parameters.AddWithValue("@identificationnumber", entity.IdentificationNumber);
        command.Parameters.AddWithValue("@phonenumber", entity.PhoneNumber);
        command.Parameters.AddWithValue("@position", entity.Position);
        command.Parameters.AddWithValue("@salary", entity.Salary);
        command.Parameters.AddWithValue("@isapproved", entity.IsApproved);

        return (int)(await command.ExecuteScalarAsync(cancellationToken) ?? throw new NpgsqlException());
    }

    public async Task<CompanyEmployeeDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = "SELECT * FROM company_employee_records WHERE id = @id";

        var command = new NpgsqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        await reader.ReadAsync(cancellationToken);

        return new CompanyEmployeeDto
        {
            Id = (int)reader["id"],
            SalaryProjectId = (int)reader["salaryprojectid"],
            FirstName = (string)reader["firstname"],
            LastName = (string)reader["lastname"],
            Email = (string)reader["email"],
            PassportSeries = (string)reader["passportseries"],
            PassportNumber = (int)reader["passportnumber"],
            IdentificationNumber = (string)reader["identificationnumber"],
            PhoneNumber = (string)reader["phonenumber"],
            Position = (string)reader["position"],
            Salary = (decimal)reader["salary"],
            IsApproved = (bool)reader["isapproved"],
        };
    }

    public async Task<ICollection<CompanyEmployeeDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = "SELECT * FROM company_employee_records";

        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var companyEmployeeDtos = new List<CompanyEmployeeDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            companyEmployeeDtos.Add(new CompanyEmployeeDto
            {
                Id = (int)reader["id"],
                SalaryProjectId = (int)reader["salaryprojectid"],
                FirstName = (string)reader["firstname"],
                LastName = (string)reader["lastname"],
                Email = (string)reader["email"],
                PassportSeries = (string)reader["passportseries"],
                PassportNumber = (int)reader["passportnumber"],
                IdentificationNumber = (string)reader["identificationnumber"],
                PhoneNumber = (string)reader["phonenumber"],
                Position = (string)reader["position"],
                Salary = (decimal)reader["salary"],
                IsApproved = (bool)reader["isapproved"]
            });
        }

        return companyEmployeeDtos;
    }

    public async Task UpdateAsync(CompanyEmployeeDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                UPDATE company_employee_records SET
                                                          salaryprojectid = @salaryprojectid,
                                                          firstname = @firstname,
                                                          lastname = @lastname,
                                                          email = @email,
                                                          passportseries = @passportseries,
                                                          passportnumber = @passportnumber,
                                                          identificationnumber = @identificationnumber,
                                                          phonenumber = @phonenumber,
                                                          position = @position,
                                                          salary = @salary,
                                                          isapproved = @isapproved
                                WHERE id = @id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@id", entity.Id);
        command.Parameters.AddWithValue("@salaryprojectid", entity.SalaryProjectId);
        command.Parameters.AddWithValue("@firstname", entity.FirstName);
        command.Parameters.AddWithValue("@lastname", entity.LastName);
        command.Parameters.AddWithValue("@email", entity.Email);
        command.Parameters.AddWithValue("@passportseries", entity.PassportSeries);
        command.Parameters.AddWithValue("@passportnumber", entity.PassportNumber);
        command.Parameters.AddWithValue("@identificationnumber", entity.IdentificationNumber);
        command.Parameters.AddWithValue("@phonenumber", entity.PhoneNumber);
        command.Parameters.AddWithValue("@position", entity.Position);
        command.Parameters.AddWithValue("@salary", entity.Salary);
        command.Parameters.AddWithValue("@isapproved", entity.IsApproved);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = "DELETE FROM company_employee_records WHERE id = @id";

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@id", id);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<ICollection<CompanyEmployeeDto>> GetCompanyEmployeesBySalaryProjectIdAsync(int salaryProjectId,
        CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM company_employee_records WHERE salaryprojectid = @salaryprojectid
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@salaryprojectid", salaryProjectId);
        
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        var companyEmployeeDtos = new List<CompanyEmployeeDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            companyEmployeeDtos.Add(new CompanyEmployeeDto
            {
                Id = (int)reader["id"],
                SalaryProjectId = (int)reader["salaryprojectid"],
                FirstName = (string)reader["firstname"],
                LastName = (string)reader["lastname"],
                Email = (string)reader["email"],
                PassportSeries = (string)reader["passportseries"],
                PassportNumber = (int)reader["passportnumber"],
                IdentificationNumber = (string)reader["identificationnumber"],
                PhoneNumber = (string)reader["phonenumber"],
                Position = (string)reader["position"],
                Salary = (decimal)reader["salary"],
                IsApproved = (bool)reader["isapproved"]
            });
        }

        return companyEmployeeDtos;
    }
}