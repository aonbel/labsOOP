using Domain.Dtos.BankServiceDtos;
using Domain.Interfaces.IRepositories.IBankServiceRepositories;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories.BankServiceRepositories;

public class SalaryProjectRepository(IOptions<PostgresOptions> options) : ISalaryProjectRepository
{
    private readonly string _connectionString = options.Value.GetConnectionString();

    public async Task<int> AddAsync(SalaryProjectDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                INSERT INTO salary_project_records  
                                (bankrecordid, lastupdatedat, createdat, closedat, terminmonths, isapproved)
                                VALUES
                                (@bankrecordid, @lastupdatedat, @createdat, @closedat, @terminmonths, @isapproved)
                                RETURNING id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

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
                                SELECT * FROM salary_project_records WHERE id = @id;
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        await reader.ReadAsync(cancellationToken);

        return new SalaryProjectDto
        {
            Id = (int)reader["id"],
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

    public async Task UpdateAsync(SalaryProjectDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                UPDATE salary_project_records
                                SET
                                    bankrecordid = @bankrecordid,
                                    lastupdatedat = @lastupdatedat,
                                    createdat = @createdat,
                                    closedat = @closedat,
                                    terminmonths = @terminmonths,
                                    isapproved = @isapproved
                                WHERE id = @id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@id", entity.Id);
        command.Parameters.AddWithValue("@bankrecordid", entity.BankRecordId);
        command.Parameters.AddWithValue("@lastupdatedat", entity.LastUpdatedAt);
        command.Parameters.AddWithValue("@createdat", entity.CreatedAt);
        command.Parameters.AddWithValue("@closedat", entity.ClosedAt);
        command.Parameters.AddWithValue("@terminmonths", entity.TermInMonths);
        command.Parameters.AddWithValue("@isapproved", entity.IsApproved);

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

    public async Task<ICollection<SalaryProjectDto>> GetByBankRecordIdAsync(int bankRecordId, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM salary_project_records WHERE bankrecordid = @bankRecordId;
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@bankrecordid", bankRecordId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var salaryProjectDtos = new List<SalaryProjectDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            salaryProjectDtos.Add(new SalaryProjectDto
            {
                Id = (int)reader["id"],
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

    public async Task<ICollection<SalaryProjectDto>> GetAllNotApprovedAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM salary_project_records WHERE isapproved = FALSE
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var salaryProjectDtos = new List<SalaryProjectDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            salaryProjectDtos.Add(new SalaryProjectDto
            {
                Id = (int)reader["id"],
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