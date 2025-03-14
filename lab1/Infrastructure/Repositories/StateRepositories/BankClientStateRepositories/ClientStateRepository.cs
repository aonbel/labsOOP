using Domain.Dtos.BankClientDtos;
using Domain.Interfaces.IRepositories;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories.StateRepositories.BankClientStateRepositories;

public class ClientStateRepository(IOptions<PostgresOptions> options) : ICRRepository<ClientDto>
{
    private readonly string _connectionString = options.Value.GetConnectionString();
    
    public async Task<int> AddAsync(ClientDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                INSERT INTO client_state_records
                                (clientid, firstname, lastname, email, passportseries, passportnumber, identificationnumber, phonenumber, isapproved) 
                                VALUES
                                (@clientid, @firstname, @lastname, @email, @passportseries, @passportnumber, @identificationnumber, @phonenumber, @isapproved)
                                RETURNING id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@clientid", entity.Id);
        command.Parameters.AddWithValue("@firstname", entity.FirstName);
        command.Parameters.AddWithValue("@lastname", entity.LastName);
        command.Parameters.AddWithValue("@email", entity.Email);
        command.Parameters.AddWithValue("@passportseries", entity.PassportSeries);
        command.Parameters.AddWithValue("@passportnumber", entity.PassportNumber);
        command.Parameters.AddWithValue("@identificationnumber", entity.IdentificationNumber);
        command.Parameters.AddWithValue("@phonenumber", entity.PhoneNumber);
        command.Parameters.AddWithValue("@isapproved", entity.IsApproved);

        return (int)(await command.ExecuteScalarAsync(cancellationToken) ?? throw new NpgsqlException());
    }

    public async Task<ClientDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = "SELECT * FROM client_state_records WHERE id = @id";

        var command = new NpgsqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        await reader.ReadAsync(cancellationToken);

        return new ClientDto
        {
            Id = (int)reader["clientid"],
            FirstName = (string)reader["firstname"],
            LastName = (string)reader["lastname"],
            Email = (string)reader["email"],
            PassportSeries = (string)reader["passportseries"],
            PassportNumber = (int)reader["passportnumber"],
            IdentificationNumber = (string)reader["identificationnumber"],
            PhoneNumber = (string)reader["phonenumber"],
            IsApproved = (bool)reader["isapproved"]
        };
    }

    public async Task<ICollection<ClientDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = "SELECT * FROM client_state_records";

        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var clientDtos = new List<ClientDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            clientDtos.Add(new ClientDto
            {
                Id = (int)reader["clientid"],
                FirstName = (string)reader["firstname"],
                LastName = (string)reader["lastname"],
                Email = (string)reader["email"],
                PassportSeries = (string)reader["passportseries"],
                PassportNumber = (int)reader["passportnumber"],
                IdentificationNumber = (string)reader["identificationnumber"],
                PhoneNumber = (string)reader["phonenumber"],
                IsApproved = (bool)reader["isapproved"]
            });
        }

        return clientDtos;
    }
}