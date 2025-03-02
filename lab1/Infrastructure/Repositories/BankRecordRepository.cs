using Infrastructure.Interfaces;
using Infrastructure.Dtos;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories;

public class BankRecordRepository(IOptions<PostgresOptions> options) : IBankRecordRepository
{
    private readonly string _connectionString = options.Value.GetConnectionString();

    public async Task<int> AddAsync(BankRecordDto entity, CancellationToken cancellationToken)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                INSERT INTO bank_record_records
                                (name, amount, isactive, clientid, companyid, companyemployeeid, bankid) 
                                VALUES 
                                (@name, @amount, @isactive, @clientid, @companyid, @companyemployeeid, @bankid)
                                RETURNING id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@name", entity.Name);
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
                                SELECT * FROM bank_record_records WHERE id = @id
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
            IsActive = (bool)reader["isactive"],
            ClientId = (int)reader["clientid"],
            CompanyId = (int)reader["companyid"],
            CompanyEmployeeId = (int)reader["companyemployeeid"],
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
                Id = (int)reader["id"],
                Name = (string)reader["name"],
                Amount = (decimal)reader["amount"],
                IsActive = (bool)reader["isactive"],
                ClientId = (int)reader["clientid"],
                CompanyId = (int)reader["companyid"],
                CompanyEmployeeId = (int)reader["companyemployeeid"],
                BankId = (int)reader["bankid"]
            });
        }

        return records;
    }

    public async Task UpdateAsync(BankRecordDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                UPDATE bank_record_records SET 
                                                        name = @name,
                                                        amount = @amount,
                                                        isactive = @isactive,
                                                        clientid = @clientid,
                                                        companyid = @companyid,
                                                        companyemployeeid = @companyemployeeid
                                                        bankid = @bankid
                                                    WHERE id = @id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@id", entity.Id);
        command.Parameters.AddWithValue("@name", entity.Name);
        command.Parameters.AddWithValue("@amount", entity.Amount);
        command.Parameters.AddWithValue("@isactive", entity.IsActive);
        command.Parameters.AddWithValue("@clientid", entity.ClientId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@companyid", entity.CompanyId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@companyemployeeid", entity.CompanyEmployeeId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@bankid", entity.BankId);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                DELETE FROM bank_record_records
                                       WHERE id = @id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@id", id);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<ICollection<BankRecordDto>> GetAllBankRecordsByClientIdAsync(int clientId,
        CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM bank_record_records
                                WHERE clientid = @clientid
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@clientid", clientId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var bankRecordDtos = new List<BankRecordDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            bankRecordDtos.Add(new BankRecordDto
            {
                Id = (int)reader["id"],
                Name = (string)reader["name"],
                Amount = (decimal)reader["amount"],
                IsActive = (bool)reader["isactive"],
                ClientId = (int)reader["clientid"],
                CompanyId = (int)reader["companyid"],
                CompanyEmployeeId = (int)reader["companyemployeeid"],
                BankId = (int)reader["bankid"]
            });
        }

        return bankRecordDtos;
    }

    public async Task<ICollection<BankRecordDto>> GetAllBankRecordsByCompanyIdAsync(int companyId,
        CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM bank_record_records
                                WHERE companyid = @companyid
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@companyid", companyId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var bankRecordDtos = new List<BankRecordDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            bankRecordDtos.Add(new BankRecordDto
            {
                Id = (int)reader["id"],
                Name = (string)reader["name"],
                Amount = (decimal)reader["amount"],
                IsActive = (bool)reader["isactive"],
                ClientId = (int)reader["clientid"],
                CompanyId = (int)reader["companyid"],
                CompanyEmployeeId = (int)reader["companyemployeeid"],
                BankId = (int)reader["bankid"]
            });
        }

        return bankRecordDtos;
    }

    public async Task<ICollection<BankRecordDto>> GetAllBankRecordsByCompanyEmployeeIdAsync(int companyEmployeeId,
        CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM bank_record_records
                                WHERE companyemployeeid = @companyemployeeid
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@companyemployeeid", companyEmployeeId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var bankRecordDtos = new List<BankRecordDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            bankRecordDtos.Add(new BankRecordDto
            {
                Id = (int)reader["id"],
                Name = (string)reader["name"],
                Amount = (decimal)reader["amount"],
                IsActive = (bool)reader["isactive"],
                ClientId = (int)reader["clientid"],
                CompanyId = (int)reader["companyid"],
                CompanyEmployeeId = (int)reader["companyemployeeid"],
                BankId = (int)reader["bankid"]
            });
        }

        return bankRecordDtos;
    }
}