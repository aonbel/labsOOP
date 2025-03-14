using Domain.Dtos.BankServiceDtos;
using Domain.Interfaces.IRepositories;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories.StateRepositories.BankServiceStateRepositories;

public class DepositStateRepository(IOptions<PostgresOptions> options) : ICRRepository<DepositDto>
{
    private readonly string _connectionString = options.Value.GetConnectionString();
    
    public async Task<int> AddAsync(DepositDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                INSERT INTO deposit_state_records 
                                (depositid, bankrecordid, interestrate, isinteractable, lastupdatedat, createdat, closedat, terminmonths, isapproved) 
                                VALUES
                                (@depositid, @bankrecordid, @interestrate, @isinteractable, @lastupdatedat, @createdat, @closedat, @terminmonths, @isapproved)
                                RETURNING id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@depositid", entity.Id);
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
                                SELECT * FROM deposit_state_records 
                                         WHERE id = @id;
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        await reader.ReadAsync(cancellationToken);

        return new DepositDto
        {
            Id = (int)reader["depositid"],
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
                                SELECT * FROM deposit_state_records;
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var depositDtos = new List<DepositDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            depositDtos.Add(new DepositDto
            {
                Id = (int)reader["depositid"],
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
}