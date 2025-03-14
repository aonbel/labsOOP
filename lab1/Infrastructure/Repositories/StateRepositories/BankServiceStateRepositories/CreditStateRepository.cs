using Domain.Dtos.BankServiceDtos;
using Domain.Interfaces.IRepositories;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories.StateRepositories.BankServiceStateRepositories;

public class CreditStateRepository (IOptions<PostgresOptions> options) : ICRRepository<CreditDto>
{
    private readonly string _connectionString = options.Value.GetConnectionString();
    public async Task<int> AddAsync(CreditDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                INSERT INTO credit_state_records 
                                (creditid, bankrecordid, interestrate, amount, lastupdatedat, createdat, closedat, terminmonths, isapproved) 
                                VALUES
                                (@creditid, @bankrecordid, @interestrate, @amount, @lastupdatedat, @createdat, @closedat, @terminmonths, @isapproved)
                                RETURNING id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@creditid", entity.Id);
        command.Parameters.AddWithValue("@bankrecordid", entity.BankRecordId);
        command.Parameters.AddWithValue("@interestrate", entity.InterestRate);
        command.Parameters.AddWithValue("@amount", entity.Amount);
        command.Parameters.AddWithValue("@lastupdatedat", entity.LastUpdatedAt);
        command.Parameters.AddWithValue("@createdat", entity.CreatedAt);
        command.Parameters.AddWithValue("@closedat", entity.ClosedAt);
        command.Parameters.AddWithValue("@terminmonths", entity.TermInMonths);
        command.Parameters.AddWithValue("@isapproved", entity.IsApproved);

        return (int)(await command.ExecuteScalarAsync(cancellationToken) ?? throw new NpgsqlException());
    }

    public async Task<CreditDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM credit_state_records 
                                         WHERE id = @id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        await reader.ReadAsync(cancellationToken);

        return new CreditDto
        {
            Id = (int)reader["creditid"],
            BankRecordId = (int)reader["bankrecordid"],
            InterestRate = (decimal)reader["interestrate"],
            Amount = (decimal)reader["amount"],
            LastUpdatedAt = (DateTime)reader["lastupdatedat"],
            CreatedAt = (DateTime)reader["createdat"],
            ClosedAt = (DateTime)reader["closedat"],
            TermInMonths = (int)reader["terminmonths"],
            IsApproved = (bool)reader["isapproved"]
        };
    }

    public async Task<ICollection<CreditDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM credit_state_records;
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var creditDtos = new List<CreditDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            creditDtos.Add(new CreditDto
            {
                Id = (int)reader["creditid"],
                BankRecordId = (int)reader["bankrecordid"],
                InterestRate = (decimal)reader["interestrate"],
                Amount = (decimal)reader["amount"],
                LastUpdatedAt = (DateTime)reader["lastupdatedat"],
                CreatedAt = (DateTime)reader["createdat"],
                ClosedAt = (DateTime)reader["closedat"],
                TermInMonths = (int)reader["terminmonths"],
                IsApproved = (bool)reader["isapproved"]
            });
        }

        return creditDtos;
    }
}