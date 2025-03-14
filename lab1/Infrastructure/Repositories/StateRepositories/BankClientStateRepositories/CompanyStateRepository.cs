using Domain.Dtos.BankClientDtos;
using Domain.Entities.BankClients;
using Domain.Interfaces.IRepositories;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories.StateRepositories.BankClientStateRepositories;

public class CompanyStateRepository(IOptions<PostgresOptions> options) : ICRRepository<CompanyDto>
{
    private readonly string _connectionString = options.Value.GetConnectionString();

    public async Task<int> AddAsync(CompanyDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                INSERT INTO company_state_records
                                (companyid, companytype, taxidentificationnumber, taxidentificationtype, address, isapproved) 
                                VALUES
                                (@companyid, @companytype, @taxidentificationnumber, @taxidentificationtype, @address, @isapproved)
                                RETURNING id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@companyid", entity.Id);
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
                                SELECT * FROM company_state_records 
                                         WHERE id = @id
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
                                SELECT * FROM company_state_records
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var companies = new List<CompanyDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            companies.Add(new CompanyDto
            {
                Id = (int)reader["companyid"],
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