using Domain.Dtos.BankServiceDtos;
using Domain.Interfaces.IRepositories;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories.StateRepositories.BankServiceStateRepositories;

public class SalaryProjectStateRepository(IOptions<PostgresOptions> options) : ICRRepository<SalaryProjectDto>
{
    private readonly string _connectionString = options.Value.GetConnectionString();
    
    public async Task<int> AddAsync(SalaryProjectDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                INSERT INTO salary_project_state_records  
                                (salaryprojectid, bankrecordid, lastupdatedat, createdat, closedat, terminmonths, isapproved)
                                VALUES
                                (@salaryprojectid, @bankrecordid, @lastupdatedat, @createdat, @closedat, @terminmonths, @isapproved)
                                RETURNING id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@salaryprojectid", entity.Id);
        command.Parameters.AddWithValue("@bankrecordid", entity.BankRecordId);
        command.Parameters.AddWithValue("@lastupdatedat", entity.LastUpdatedAt);
        command.Parameters.AddWithValue("@createdat", entity.CreatedAt);
        command.Parameters.AddWithValue("@closedat", entity.ClosedAt);
        command.Parameters.AddWithValue("@terminmonths", entity.TermInMonths);
        command.Parameters.AddWithValue("@isapproved", entity.IsApproved);

        return (int)(await command.ExecuteScalarAsync(cancellationToken) ?? throw new NpgsqlException());
    }

    public async Task<SalaryProjectDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM salary_project_state_records WHERE id = @id;
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        await reader.ReadAsync(cancellationToken);

        return new SalaryProjectDto
        {
            Id = (int)reader["salaryprojectid"],
            BankRecordId = (int)reader["bankrecordid"],
            LastUpdatedAt = (DateTime)reader["lastupdatedat"],
            CreatedAt = (DateTime)reader["createdat"],
            ClosedAt = (DateTime)reader["closedat"],
            TermInMonths = (int)reader["terminmonths"],
            IsApproved = (bool)reader["isapproved"]
        };
    }

    public async Task<ICollection<SalaryProjectDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM salary_project_state_records;
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var salaryProjectDtos = new List<SalaryProjectDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            salaryProjectDtos.Add(new SalaryProjectDto
            {
                Id = (int)reader["salaryprojectid"],
                BankRecordId = (int)reader["bankrecordid"],
                LastUpdatedAt = (DateTime)reader["lastupdatedat"],
                CreatedAt = (DateTime)reader["createdat"],
                ClosedAt = (DateTime)reader["closedat"],
                TermInMonths = (int)reader["terminmonths"],
                IsApproved = (bool)reader["isapproved"]
            });
        }

        return salaryProjectDtos;
    }
}