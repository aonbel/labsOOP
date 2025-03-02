using Domain.Entities.BankClients;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories;

public class BankRepository(IOptions<PostgresOptions> options) : IRepository<BankDto>
{
    private readonly string _connectionString = options.Value.GetConnectionString();

    public async Task<int> AddAsync(BankDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                INSERT INTO bank_records
                                (companytype, taxidentificationnumber, taxidentificationtype, address, bankidentificationcode) 
                                VALUES
                                (@companytype, @taxidentificationnumber, @taxidentificationtype, @address, @bankidentificationcode)
                                RETURNING id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@companytype", (int)entity.CompanyType);
        command.Parameters.AddWithValue("@taxidentificationnumber", entity.TaxIdentificationNumber);
        command.Parameters.AddWithValue("@taxidentificationtype", entity.TaxIdentificationType);
        command.Parameters.AddWithValue("@address", entity.Address);
        command.Parameters.AddWithValue("@bankidentificationcode", entity.BankIdentificationCode);

        return (int)(await command.ExecuteScalarAsync(cancellationToken) ??
                     throw new ArgumentException(null, nameof(entity)));
    }

    public async Task<BankDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT *
                                FROM bank_records
                                WHERE id = @id
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        await reader.ReadAsync(cancellationToken);

        return new BankDto
        {
            Id = (int)reader["id"],
            CompanyType = (CompanyType)(int)reader["companytype"],
            TaxIdentificationNumber = (string)reader["taxidentificationnumber"],
            TaxIdentificationType = (string)reader["taxidentificationtype"],
            Address = (string)reader["address"],
            BankIdentificationCode = (string)reader["bankidentificationcode"]
        };
    }

    public async Task<ICollection<BankDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT *
                                FROM bank_records
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        await reader.ReadAsync(cancellationToken);
        
        var bankDtos = new List<BankDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            bankDtos.Add(new BankDto
            {
                Id = (int)reader["id"],
                CompanyType = (CompanyType)(int)reader["companytype"],
                TaxIdentificationNumber = (string)reader["taxidentificationnumber"],
                TaxIdentificationType = (string)reader["taxidentificationtype"],
                Address = (string)reader["address"],
                BankIdentificationCode = (string)reader["bankidentificationcode"]
            });
        }

        return bankDtos;
    }

    public async Task UpdateAsync(BankDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                UPDATE bank_records SET
                                                          companytype = @companytype,
                                                          taxidentificationnumber = @taxidentificationnumber,
                                                          taxidentificationtype = @taxidentificationtype,
                                                          address = @address,
                                                          bankidentificationcode = @bankidentificationcode
                                WHERE id = @id
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@id", entity.Id);
        command.Parameters.AddWithValue("@companytype", (int)entity.CompanyType);
        command.Parameters.AddWithValue("@taxidentificationnumber", entity.TaxIdentificationNumber);
        command.Parameters.AddWithValue("@taxidentificationtype", entity.TaxIdentificationType);
        command.Parameters.AddWithValue("@address", entity.Address);
        command.Parameters.AddWithValue("@bankidentificationcode", entity.BankIdentificationCode);
        
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                DELETE FROM company_records WHERE id = @id
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@id", id);
        
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}