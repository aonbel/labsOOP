using Infrastructure.Interfaces;
using Infrastructure.Dtos;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories;

public class BankRecordRepository(IOptions<PostgresOptions> options) : IRepository<BankRecordDto>
{
    private readonly string _connectionString = options.Value.GetConnectionString();

    public async Task<int> AddAsync(BankRecordDto entity, CancellationToken cancellationToken)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                    INSERT INTO bank_records
                                    (name, amount, isactive, bankuserid) 
                                VALUES 
                                    (@name, @amount, @isactive, @bankuserid) 
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@name", entity.Name);
        command.Parameters.AddWithValue("@amount", entity.Amount);
        command.Parameters.AddWithValue("@isactive", entity.IsActive);
        command.Parameters.AddWithValue("@bankuserid", entity.BankClientId);

        var bankRecordId = (int)(await command.ExecuteScalarAsync(cancellationToken) ?? throw new NpgsqlException());

        return bankRecordId;
    }

    public async Task<BankRecordDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM bank_records WHERE id = @id
                                """;
        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        await reader.ReadAsync(cancellationToken);

        return new BankRecordDto
        {
            Id = (int)reader["id"],
            Name = (string)reader["name"],
            Amount = (decimal)reader["amount"],
            IsActive = (bool)reader["isactive"]
        };
    }

    public async Task<ICollection<BankRecordDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM bank_records
                                """;
        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var records = new List<BankRecordDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            records.Add(new BankRecordDto
            {
                Id = (int)reader["id"],
                Name = (string)reader["name"],
                Amount = (decimal)reader["amount"],
                IsActive = (bool)reader["isactive"]
            });
        }

        return records;
    }

    public async Task UpdateAsync(BankRecordDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                UPDATE bank_records SET 
                                                        name = @name,
                                                        amount = @amount,
                                                        isactive = @isactive 
                                                    WHERE id = @id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@id", entity.Id);
        command.Parameters.AddWithValue("@name", entity.Name);
        command.Parameters.AddWithValue("@amount", entity.Amount);
        command.Parameters.AddWithValue("@isactive", entity.IsActive);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                DELETE FROM bank_records
                                       WHERE id = @id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@id", id);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}