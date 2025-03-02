using Infrastructure.Dtos;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories;

public class InstallmentRepository(IOptions<PostgresOptions> options) : IInstallmentRepository
{
    private readonly string _connectionString = options.Value.GetConnectionString();

    public async Task<int> AddAsync(InstallmentDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                INSERT INTO installment_records 
                                (bankrecordid, name, interestrate, amount, lastupdatedat, createdat, closedat, terminmonths) 
                                VALUES
                                (@bankrecordid, @name, @interestrate, @amount, @lastupdatedat, @createdat, @closedat, @terminmonths)
                                RETURNING id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@bankrecordid", entity.BankRecordId);
        command.Parameters.AddWithValue("@name", entity.Name);
        command.Parameters.AddWithValue("@interestrate", entity.InterestRate);
        command.Parameters.AddWithValue("@amount", entity.Amount);
        command.Parameters.AddWithValue("@lastupdatedat", entity.LastUpdatedAt);
        command.Parameters.AddWithValue("@createdat", entity.CreatedAt);
        command.Parameters.AddWithValue("@closedat", entity.ClosedAt);
        command.Parameters.AddWithValue("@terminmonths", entity.TermInMonths);

        return (int)(await command.ExecuteScalarAsync(cancellationToken) ?? throw new NpgsqlException());
    }

    public async Task<InstallmentDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM installment_records WHERE id = @id;
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        await reader.ReadAsync(cancellationToken);

        return new InstallmentDto
        {
            Id = (int)reader["id"],
            BankRecordId = (int)reader["bankrecordid"],
            Name = (string)reader["name"],
            InterestRate = (decimal)reader["interestrate"],
            Amount = (decimal)reader["amount"],
            LastUpdatedAt = (DateTime)reader["lastupdatedat"],
            CreatedAt = (DateTime)reader["createdat"],
            ClosedAt = (DateTime)reader["closedat"],
            TermInMonths = (int)reader["terminmonths"]
        };
    }

    public async Task<ICollection<InstallmentDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM installment_records;
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var installments = new List<InstallmentDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            installments.Add(new InstallmentDto
            {
                Id = (int)reader["id"],
                BankRecordId = (int)reader["bankrecordid"],
                Name = (string)reader["name"],
                InterestRate = (decimal)reader["interestrate"],
                Amount = (decimal)reader["amount"],
                LastUpdatedAt = (DateTime)reader["lastupdatedat"],
                CreatedAt = (DateTime)reader["createdat"],
                ClosedAt = (DateTime)reader["closedat"],
                TermInMonths = (int)reader["terminmonths"]
            });
        }

        return installments;
    }

    public async Task UpdateAsync(InstallmentDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                UPDATE installment_records SET
                                                          bankrecordid = @bankrecordid,
                                                          name = @name,
                                                          interestrate = @interestrate,
                                                          amount = @amount,
                                                          lastupdatedat = @lastupdatedat,
                                                          createdat = @createdat,
                                                          closedat = @closedat,
                                                          terminmonths = @terminmonths
                                WHERE id = @id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@id", entity.Id);
        command.Parameters.AddWithValue("@bankrecordid", entity.BankRecordId);
        command.Parameters.AddWithValue("@name", entity.Name);
        command.Parameters.AddWithValue("@interestrate", entity.InterestRate);
        command.Parameters.AddWithValue("@amount", entity.Amount);
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
                                DELETE FROM installment_records WHERE id = @id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@id", id);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public Task<ICollection<InstallmentDto>> GetByBankRecordIdAsync(int bankRecordId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}