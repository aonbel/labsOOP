using Domain.Entities.BankServices;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories;

public class SalaryProjectRepository(IOptions<PostgresOptions> options) : ISalaryProjectRepository
{
    private readonly string _connectionString = options.Value.GetConnectionString();

    public async Task<int> AddAsync(SalaryProjectDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                INSERT INTO salary_project_records  
                                (bankrecordid, name, lastupdatedat, createdat, closedat, terminmonths)
                                VALUES
                                (@bankrecordid, @name, @lastupdatedat, @createdat, @closedat, @terminmonths)
                                RETURNING id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@bankrecordid", entity.BankRecordId);
        command.Parameters.AddWithValue("@name", entity.Name);
        command.Parameters.AddWithValue("@lastupdatedat", entity.LastUpdatedAt);
        command.Parameters.AddWithValue("@createdat", entity.CreatedAt);
        command.Parameters.AddWithValue("@closedat", entity.ClosedAt);
        command.Parameters.AddWithValue("@terminmonths", entity.TermInMonths);

        return (int)(await command.ExecuteScalarAsync(cancellationToken) ?? throw new NpgsqlException());
    }

    public async Task<SalaryProjectDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM salary_project_records WHERE id = @id;
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        await reader.ReadAsync(cancellationToken);

        return new SalaryProjectDto
        {
            Id = (int)reader["id"],
            Name = (string)reader["name"],
            BankRecordId = (int)reader["bankrecordid"],
            LastUpdatedAt = (DateTime)reader["lastupdatedat"],
            CreatedAt = (DateTime)reader["createdat"],
            ClosedAt = (DateTime)reader["closedat"],
            TermInMonths = (int)reader["terminmonths"]
        };
    }

    public async Task<ICollection<SalaryProjectDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM salary_project_records;
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var salaryProjectDtos = new List<SalaryProjectDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            salaryProjectDtos.Add(new SalaryProjectDto
            {
                Id = (int)reader["id"],
                Name = (string)reader["name"],
                BankRecordId = (int)reader["bankrecordid"],
                LastUpdatedAt = (DateTime)reader["lastupdatedat"],
                CreatedAt = (DateTime)reader["createdat"],
                ClosedAt = (DateTime)reader["closedat"],
                TermInMonths = (int)reader["terminmonths"]
            });
        }

        return salaryProjectDtos;
    }

    public async Task UpdateAsync(SalaryProjectDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                UPDATE salary_project_records
                                SET
                                    name = @name,
                                    bankrecordid = @bankrecordid,
                                    lastupdatedat = @lastupdatedat,
                                    createdat = @createdat,
                                    closedat = @closedat,
                                    terminmonths = @terminmonths
                                WHERE id = @id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@id", entity.Id);
        command.Parameters.AddWithValue("@name", entity.Name);
        command.Parameters.AddWithValue("@bankrecordid", entity.BankRecordId);
        command.Parameters.AddWithValue("@lastupdatedat", entity.LastUpdatedAt);
        command.Parameters.AddWithValue("@createdat", entity.CreatedAt);
        command.Parameters.AddWithValue("@closedat", entity.ClosedAt);
        command.Parameters.AddWithValue("@terminmonths", entity.TermInMonths);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                DELETE FROM salary_project_records WHERE id = @id
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@id", id);
        
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public Task<ICollection<SalaryProjectDto>> GetByBankRecordIdAsync(int bankRecordId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}