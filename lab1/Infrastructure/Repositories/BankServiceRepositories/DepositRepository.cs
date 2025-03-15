using Domain.Dtos.BankServiceDtos;
using Domain.Interfaces.IRepositories.IBankServiceRepositories;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories.BankServiceRepositories;

public class DepositRepository(IOptions<PostgresOptions> options) : IDepositRepository
{
    private readonly string _connectionString = options.Value.GetConnectionString();

    public async Task<int> AddAsync(DepositDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                INSERT INTO deposit_records 
                                (bankrecordid, interestrate, isinteractable, lastupdatedat, createdat, closedat, terminmonths, isapproved) 
                                VALUES
                                (@bankrecordid, @interestrate, @isinteractable, @lastupdatedat, @createdat, @closedat, @terminmonths, @isapproved)
                                RETURNING id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@bankrecordid", entity.BankRecordId);
        command.Parameters.AddWithValue("@interestrate", entity.InterestRate);
        command.Parameters.AddWithValue("@isinteractable", entity.IsInteractable);
        command.Parameters.AddWithValue("@lastupdatedat", entity.LastUpdatedAt);
        command.Parameters.AddWithValue("@createdat", entity.CreatedAt);
        command.Parameters.AddWithValue("@closedat", entity.ClosedAt);
        command.Parameters.AddWithValue("@terminmonths", entity.TermInMonths);
        command.Parameters.AddWithValue("@isapproved", entity.IsApproved);

        return (int)(await command.ExecuteScalarAsync(cancellationToken) ?? throw new NpgsqlException());
    }

    public async Task<DepositDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM deposit_records WHERE id = @id;
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        await reader.ReadAsync(cancellationToken);

        return new DepositDto
        {
            Id = (int)reader["id"],
            BankRecordId = (int)reader["bankrecordid"],
            InterestRate = (decimal)reader["interestrate"],
            IsInteractable = (bool)reader["isinteractable"],
            LastUpdatedAt = (DateTime)reader["lastupdatedat"],
            CreatedAt = (DateTime)reader["createdat"],
            ClosedAt = (DateTime)reader["closedat"],
            TermInMonths = (int)reader["terminmonths"],
            IsApproved = (bool)reader["isapproved"]
        };
    }

    public async Task<ICollection<DepositDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM deposit_records;
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var depositDtos = new List<DepositDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            depositDtos.Add(new DepositDto
            {
                Id = (int)reader["id"],
                BankRecordId = (int)reader["bankrecordid"],
                InterestRate = (decimal)reader["interestrate"],
                IsInteractable = (bool)reader["isinteractable"],
                LastUpdatedAt = (DateTime)reader["lastupdatedat"],
                CreatedAt = (DateTime)reader["createdat"],
                ClosedAt = (DateTime)reader["closedat"],
                TermInMonths = (int)reader["terminmonths"],
                IsApproved = (bool)reader["isapproved"]
            });
        }

        return depositDtos;
    }

    public async Task UpdateAsync(DepositDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                UPDATE deposit_records SET
                                                          bankrecordid = @bankrecordid,
                                                          interestrate = @interestrate,
                                                          isinteractable = @isinteractable,
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
        command.Parameters.AddWithValue("@interestrate", entity.InterestRate);
        command.Parameters.AddWithValue("@isinteractable", entity.IsInteractable);
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
                                DELETE FROM deposit_records WHERE id = @id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@id", id);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<ICollection<DepositDto>> GetByBankRecordIdAsync(int bankRecordId, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM deposit_records WHERE bankrecordid = @bankrecordid
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@bankrecordid", bankRecordId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var depositDtos = new List<DepositDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            depositDtos.Add(new DepositDto
            {
                Id = (int)reader["id"],
                BankRecordId = (int)reader["bankrecordid"],
                InterestRate = (decimal)reader["interestrate"],
                LastUpdatedAt = (DateTime)reader["lastupdatedat"],
                CreatedAt = (DateTime)reader["createdat"],
                ClosedAt = (DateTime)reader["closedat"],
                TermInMonths = (int)reader["terminmonths"],
                IsApproved = (bool)reader["isapproved"]
            });
        }

        return depositDtos;
    }

    public async Task<ICollection<DepositDto>> GetAllNotApprovedAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM deposit_records WHERE isapproved = FALSE;
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var depositDtos = new List<DepositDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            depositDtos.Add(new DepositDto
            {
                Id = (int)reader["id"],
                BankRecordId = (int)reader["bankrecordid"],
                InterestRate = (decimal)reader["interestrate"],
                LastUpdatedAt = (DateTime)reader["lastupdatedat"],
                CreatedAt = (DateTime)reader["createdat"],
                ClosedAt = (DateTime)reader["closedat"],
                TermInMonths = (int)reader["terminmonths"],
                IsApproved = (bool)reader["isapproved"]
            });
        }

        return depositDtos;
    }
}