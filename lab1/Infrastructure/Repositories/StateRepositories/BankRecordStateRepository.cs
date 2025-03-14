using Domain.Dtos;
using Domain.Interfaces.IRepositories;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories.StateRepositories;

public class BankRecordStateRepository(IOptions<PostgresOptions> options) : ICRRepository<BankRecordDto>
{
    private readonly string _connectionString = options.Value.GetConnectionString();

    public async Task<int> AddAsync(BankRecordDto entity, CancellationToken cancellationToken)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                INSERT INTO bank_record_state_records
                                (bankrecordid, amount, isactive, clientid, companyid, companyemployeeid, bankid) 
                                VALUES 
                                (@bankrecordid, @amount, @isactive, @clientid, @companyid, @companyemployeeid, @bankid)
                                RETURNING id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@bankrecordid", entity.Id);
        command.Parameters.AddWithValue("@amount", entity.Amount);
        command.Parameters.AddWithValue("@isactive", entity.IsActive);
        command.Parameters.AddWithValue("@clientid", entity.ClientId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@companyid", entity.CompanyId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@companyemployeeid", entity.CompanyEmployeeId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@bankid", entity.BankId);

        return (int)(await command.ExecuteScalarAsync(cancellationToken) ?? throw new NpgsqlException());
    }

    public async Task<BankRecordDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM bank_record_records 
                                         WHERE id = @id
                                """;
        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        await reader.ReadAsync(cancellationToken);

        return new BankRecordDto
        {
            Id = (int)reader["bankrecordid"],
            Amount = (decimal)reader["amount"],
            IsActive = (bool)reader["isactive"],
            ClientId = reader["clientid"] != DBNull.Value ? (int)reader["clientid"] : null,
            CompanyId = reader["companyid"] != DBNull.Value ? (int)reader["companyid"] : null,
            CompanyEmployeeId = reader["companyemployeeid"] != DBNull.Value ? (int)reader["companyemployeeid"] : null,
            BankId = (int)reader["bankid"]
        };
    }

    public async Task<ICollection<BankRecordDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM bank_record_records
                                """;
        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var records = new List<BankRecordDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            records.Add(new BankRecordDto
            {
                Id = (int)reader["bankrecordid"],
                Amount = (decimal)reader["amount"],
                IsActive = (bool)reader["isactive"],
                ClientId = reader["clientid"] != DBNull.Value ? (int)reader["clientid"] : null,
                CompanyId = reader["companyid"] != DBNull.Value ? (int)reader["companyid"] : null,
                CompanyEmployeeId = reader["companyemployeeid"] != DBNull.Value ? (int)reader["companyemployeeid"] : null,
                BankId = (int)reader["bankid"]
            });
        }

        return records;
    }
}