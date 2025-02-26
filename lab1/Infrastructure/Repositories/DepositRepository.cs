using Infrastructure.Dtos;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories;

public class DepositRepository(IOptions<PostgresOptions> options) : IRepository<DepositDto>
{
    private readonly string _connectionString = options.Value.GetConnectionString();

    public async Task<int> AddAsync(DepositDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                INSERT INTO deposit_records 
                                (bankrecordid, name, interestrate, isinteractable, lastupdatedat, createdat, closedat, terminmonths, isapproved) 
                                VALUES
                                (@bankrecordid, @name, @interestrate, @isinteractable, @lastupdatedat, @createdat, @closedat, @terminmonths, @isapproved)
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@bankrecordid", entity.BankRecordId);
        command.Parameters.AddWithValue("@name", entity.Name);
        command.Parameters.AddWithValue("@interestrate", entity.InterestRate);
        command.Parameters.AddWithValue("@isinteractable", entity.IsInteractable);
        command.Parameters.AddWithValue("@lastupdatedat", entity.LastUpdatedAt);
        command.Parameters.AddWithValue("@createdat", entity.CreatedAt);
        command.Parameters.AddWithValue("@closedat", entity.ClosedAt);
        command.Parameters.AddWithValue("@terminmonths", entity.TermInMonths);
        command.Parameters.AddWithValue("@isapproved", entity.isApproved);

        var depositId = (int)(await command.ExecuteScalarAsync(cancellationToken) ?? throw new NpgsqlException());

        return depositId;
    }

    public async Task<DepositDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM deposit_records WHERE id = @id;
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        await reader.ReadAsync(cancellationToken);

        return new DepositDto
        {
            Id = (int)reader["id"],
            BankRecordId = (int)reader["bankrecordid"],
            Name = (string)reader["name"],
            InterestRate = (decimal)reader["interestrate"],
            IsInteractable = (bool)reader["isinteractable"],
            LastUpdatedAt = (DateTime)reader["lastupdatedat"],
            CreatedAt = (DateTime)reader["createdat"],
            ClosedAt = (DateTime)reader["closedat"],
            TermInMonths = (int)reader["terminmonths"],
            isApproved = (bool)reader["isapproved"]
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

        var deposits = new List<DepositDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            deposits.Add(new DepositDto
            {
                Id = (int)reader["id"],
                BankRecordId = (int)reader["bankrecordid"],
                Name = (string)reader["name"],
                InterestRate = (decimal)reader["interestrate"],
                IsInteractable = (bool)reader["isinteractable"],
                LastUpdatedAt = (DateTime)reader["lastupdatedat"],
                CreatedAt = (DateTime)reader["createdat"],
                ClosedAt = (DateTime)reader["closedat"],
                TermInMonths = (int)reader["terminmonths"],
                isApproved = (bool)reader["isapproved"]
            });
        }

        return deposits;
    }

    public async Task UpdateAsync(DepositDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                UPDATE deposit_records SET
                                                          bankrecordid = @bankrecordid,
                                                          name = @name,
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
        command.Parameters.AddWithValue("@name", entity.Name);
        command.Parameters.AddWithValue("@interestrate", entity.InterestRate);
        command.Parameters.AddWithValue("@isinteractable", entity.IsInteractable);
        command.Parameters.AddWithValue("@lastupdatedat", entity.LastUpdatedAt);
        command.Parameters.AddWithValue("@createdat", entity.CreatedAt);
        command.Parameters.AddWithValue("@closedat", entity.ClosedAt);
        command.Parameters.AddWithValue("@terminmonths", entity.TermInMonths);
        command.Parameters.AddWithValue("@isapproved", entity.isApproved);

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
}