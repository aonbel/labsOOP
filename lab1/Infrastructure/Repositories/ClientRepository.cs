using Infrastructure.Dtos;
using Infrastructure.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories;

public class ClientRepository(IOptions<PostgresOptions> options) : IRepository<ClientDto>
{
    private readonly string _connectionString = options.Value.GetConnectionString();

    public async Task<int> AddAsync(ClientDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                INSERT INTO client_records 
                                (bankid, firstname, lastname, email, passportseries, passportnumber, identificationnumber, phonenumber) 
                                VALUES
                                (@bankid, @firstname, @lastname, @email, @passportseries, @passportnumber, @identificationnumber, @phonenumber)
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@bankid", entity.BankId);
        command.Parameters.AddWithValue("@firstname", entity.FirstName);
        command.Parameters.AddWithValue("@lastname", entity.LastName);
        command.Parameters.AddWithValue("@email", entity.Email);
        command.Parameters.AddWithValue("@passportseries", entity.PassportSeries);
        command.Parameters.AddWithValue("@passportnumber", entity.PassportNumber);
        command.Parameters.AddWithValue("@identificationnumber", entity.IdentificationNumber);
        command.Parameters.AddWithValue("@phonenumber", entity.PhoneNumber);

        var clientId = (int)(await command.ExecuteScalarAsync(cancellationToken) ?? throw new NpgsqlException());

        return clientId;
    }

    public async Task<ClientDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = "SELECT * FROM client_records WHERE id = @id";

        var command = new NpgsqlCommand(sqlQuery, connection);
        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        await reader.ReadAsync(cancellationToken);

        return new ClientDto
        {
            Id = (int)reader["id"],
            BankId = (int)reader["bankid"],
            FirstName = (string)reader["firstname"],
            LastName = (string)reader["lastname"],
            Email = (string)reader["email"],
            PassportSeries = (string)reader["passportseries"],
            PassportNumber = (int)reader["passportnumber"],
            IdentificationNumber = (string)reader["identificationnumber"],
            PhoneNumber = (string)reader["phonenumber"]
        };
    }

    public async Task<ICollection<ClientDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = "SELECT * FROM client_records";

        var command = new NpgsqlCommand(sqlQuery, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var clientDtos = new List<ClientDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            clientDtos.Add(new ClientDto
            {
                Id = (int)reader["id"],
                BankId = (int)reader["bankid"],
                FirstName = (string)reader["firstname"],
                LastName = (string)reader["lastname"],
                Email = (string)reader["email"],
                PassportSeries = (string)reader["passportseries"],
                PassportNumber = (int)reader["passportnumber"],
                IdentificationNumber = (string)reader["identificationnumber"],
                PhoneNumber = (string)reader["phonenumber"]
            });
        }

        return clientDtos;
    }

    public async Task UpdateAsync(ClientDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                UPDATE client_records SET
                                                          bankid = @bankid,
                                                          firstname = @firstname,
                                                          lastname = @lastname,
                                                          email = @email,
                                                          passportseries = @passportseries,
                                                          passportnumber = @passportnumber,
                                                          identificationnumber = @identificationnumber,
                                                          phonenumber = @phonenumber
                                WHERE id = @id
                                """;

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@id", entity.Id);
        command.Parameters.AddWithValue("@bankid", entity.BankId);
        command.Parameters.AddWithValue("@firstname", entity.FirstName);
        command.Parameters.AddWithValue("@lastname", entity.LastName);
        command.Parameters.AddWithValue("@email", entity.Email);
        command.Parameters.AddWithValue("@passportseries", entity.PassportSeries);
        command.Parameters.AddWithValue("@passportnumber", entity.PassportNumber);
        command.Parameters.AddWithValue("@identificationnumber", entity.IdentificationNumber);
        command.Parameters.AddWithValue("@phonenumber", entity.PhoneNumber);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = "DELETE FROM client_records WHERE id = @id";

        var command = new NpgsqlCommand(sqlQuery, connection);

        command.Parameters.AddWithValue("@id", id);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}