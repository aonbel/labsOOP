using Domain.Entities.BankClients;
using Infrastructure.Dtos;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories;

public class CompanyRepository(IOptions<PostgresOptions> options) : ICompanyRepository
{
    private readonly string _connectionString = options.Value.GetConnectionString();

    public async Task<int> AddAsync(CompanyDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                INSERT INTO company_records
                                (companytype, taxidentificationnumber, taxidentificationtype, address, isapproved) 
                                VALUES
                                (@companytype, @taxidentificationnumber, @taxidentificationtype, @address, @isapproved)
                                RETURNING id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@companytype", (int)entity.CompanyType);
        command.Parameters.AddWithValue("@taxidentificationnumber", entity.TaxIdentificationNumber);
        command.Parameters.AddWithValue("@taxidentificationtype", entity.TaxIdentificationType);
        command.Parameters.AddWithValue("@address", entity.Address);
        command.Parameters.AddWithValue("@isapproved", entity.IsApproved);

        return (int)(await command.ExecuteScalarAsync(cancellationToken) ?? throw new NpgsqlException());
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
            CompanyType = (CompanyType)(int)reader["companytype"],
            TaxIdentificationNumber = (string)reader["taxidentificationnumber"],
            TaxIdentificationType = (string)reader["taxidentificationtype"],
            Address = (string)reader["address"],
            IsApproved = (bool)reader["isapproved"]
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
                CompanyType = (CompanyType)(int)reader["companytype"],
                TaxIdentificationNumber = (string)reader["taxidentificationnumber"],
                TaxIdentificationType = (string)reader["taxidentificationtype"],
                Address = (string)reader["address"],
                IsApproved = (bool)reader["isapproved"]
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
                                                          companytype = @companytype,
                                                          taxidentificationnumber = @taxidentificationnumber,
                                                          taxidentificationtype = @taxidentificationtype,
                                                          address = @address,
                                                          isapproved = @isapproved
                                WHERE id = @id
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@id", entity.Id);
        command.Parameters.AddWithValue("@companytype", (int)entity.CompanyType);
        command.Parameters.AddWithValue("@taxidentificationnumber", entity.TaxIdentificationNumber);
        command.Parameters.AddWithValue("@taxidentificationtype", entity.TaxIdentificationType);
        command.Parameters.AddWithValue("@address", entity.Address);
        command.Parameters.AddWithValue("@isapproved", entity.IsApproved);
        
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

    public async Task<ICollection<CompanyDto>> GetAllNotApprovedAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM company_records WHERE isapproved = FALSE
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var companies = new List<CompanyDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            companies.Add(new CompanyDto
            {
                Id = (int)reader["id"],
                CompanyType = (CompanyType)(int)reader["companytype"],
                TaxIdentificationNumber = (string)reader["taxidentificationnumber"],
                TaxIdentificationType = (string)reader["taxidentificationtype"],
                Address = (string)reader["address"],
                IsApproved = (bool)reader["isapproved"]
            });
        }

        return companies;
    }
}