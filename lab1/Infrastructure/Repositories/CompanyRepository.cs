using Domain.Entities.BankClients;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories;

public class CompanyRepository(IOptions<PostgresOptions> options) : IRepository<CompanyDto>
{
    private readonly string _connectionString = options.Value.GetConnectionString();

    public async Task<int> AddAsync(CompanyDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                INSERT INTO company_records
                                (bankid, companytype, taxidentificationnumber, taxidentificationtype, address) 
                                VALUES
                                (@bankid, @companytype, @taxidentificationnumber, @taxidentificationtype, @address)
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@bankid", entity.BankId);
        command.Parameters.AddWithValue("@companytype", entity.CompanyType);
        command.Parameters.AddWithValue("@taxidentificationnumber", entity.TaxIdentificationNumber);
        command.Parameters.AddWithValue("@taxidentificationtype", entity.TaxIdentificationType);
        command.Parameters.AddWithValue("@address", entity.Address);

        var companyId = (int)(await command.ExecuteScalarAsync(cancellationToken) ?? throw new NpgsqlException());

        return companyId;
    }

    public async Task<CompanyDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM company_records WHERE id = @id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        return new CompanyDto
        {
            Id = (int)reader["id"],
            BankId = (int)reader["bankid"],
            CompanyType = (CompanyType)reader["companytype"], // fix
            TaxIdentificationNumber = (string)reader["taxidentificationnumber"],
            TaxIdentificationType = (string)reader["taxidentificationtype"],
            Address = (string)reader["address"]
        };
    }

    public async Task<ICollection<CompanyDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM company_records
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var companies = new List<CompanyDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            companies.Add(new CompanyDto
            {
                Id = (int)reader["id"],
                BankId = (int)reader["bankid"],
                CompanyType = (CompanyType)reader["companytype"], // fix
                TaxIdentificationNumber = (string)reader["taxidentificationnumber"],
                TaxIdentificationType = (string)reader["taxidentificationtype"],
                Address = (string)reader["address"]
            });
        }

        return companies;
    }

    public async Task UpdateAsync(CompanyDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                UPDATE company_records SET
                                                          bankid = @bankid,
                                                          companytype = @companytype,
                                                          taxidentificationnumber = @taxidentificationnumber,
                                                          taxidentificationtype = @taxidentificationtype,
                                                          address = @address
                                WHERE id = @id
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@id", entity.Id);
        command.Parameters.AddWithValue("@bankid", entity.BankId);
        command.Parameters.AddWithValue("@companytype", entity.CompanyType);
        command.Parameters.AddWithValue("@taxidentificationnumber", entity.TaxIdentificationNumber);
        command.Parameters.AddWithValue("@taxidentificationtype", entity.TaxIdentificationType);
        command.Parameters.AddWithValue("@address", entity.Address);
        
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