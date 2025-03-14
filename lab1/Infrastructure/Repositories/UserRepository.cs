using Domain.Dtos;
using Domain.Interfaces.IRepositories;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories;

public class UserRepository(IOptions<PostgresOptions> options) : IUserRepository
{
    private readonly string _connectionString = options.Value.GetConnectionString();
    
    public async Task<int> AddAsync(UserDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                INSERT INTO user_records  
                                (login, password, role, clientid, companyid, companyemployeeid)
                                VALUES
                                (@login, @password, @role, @clientid, @companyid, @companyemployeeid)
                                RETURNING id
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@login", entity.Login);
        command.Parameters.AddWithValue("@password", entity.Password);
        command.Parameters.AddWithValue("@role", entity.Role);
        command.Parameters.AddWithValue("@clientid", entity.ClientId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@companyid", entity.CompanyId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@companyemployeeid", entity.CompanyEmployeeId ?? (object)DBNull.Value);
        
        return (int)(await command.ExecuteScalarAsync(cancellationToken) ?? throw new NpgsqlException());
    }

    public async Task<UserDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM user_records 
                                WHERE id = @id
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@id", id);
        
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        await reader.ReadAsync(cancellationToken);
        
        return new UserDto
        {
            Id = (int)reader["id"],
            Login = (string)reader["login"],
            Password = (string)reader["password"],
            Role = (string)reader["role"],
            ClientId = reader["clientid"] != DBNull.Value? (int)reader["clientid"] : null,
            CompanyId = reader["companyid"] != DBNull.Value? (int)reader["companyid"] : null,
            CompanyEmployeeId = reader["companyemployeeid"] != DBNull.Value? (int)reader["companyemployeeid"] : null,
        };
    }

    public async Task<ICollection<UserDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM user_records
                                WHERE login = @login
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        var userDtos = new List<UserDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            userDtos.Add(new UserDto
            {
                Id = (int)reader["id"],
                Login = (string)reader["login"],
                Password = (string)reader["password"],
                Role = (string)reader["role"],
                ClientId = reader["clientid"] != DBNull.Value? (int)reader["clientid"] : null,
                CompanyId = reader["companyid"] != DBNull.Value? (int)reader["companyid"] : null,
                CompanyEmployeeId = reader["companyemployeeid"] != DBNull.Value? (int)reader["companyemployeeid"] : null,
            });
        }
        
        return userDtos;
    }

    public async Task UpdateAsync(UserDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                UPDATE user_records
                                SET
                                    login = @login,
                                    password = @password,
                                    role = @role,
                                    clientid = @clientid,
                                    companyid = @companyid,
                                    companyemployeeid = @companyemployeeid
                                WHERE id = @id
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@id", entity.Id);
        command.Parameters.AddWithValue("@login", entity.Login);
        command.Parameters.AddWithValue("@password", entity.Password);
        command.Parameters.AddWithValue("@role", entity.Role);
        command.Parameters.AddWithValue("@clientid", entity.ClientId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@companyid", entity.CompanyId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@companyemployeeid", entity.CompanyEmployeeId ?? (object)DBNull.Value);
        
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                DELETE FROM user_records
                                WHERE id = @id
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@id", id);
        
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<UserDto> GetByLoginAsync(string login, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM user_records
                                WHERE login = @login
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@login", login);
        
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        await reader.ReadAsync(cancellationToken);

        return new UserDto
        {
            Id = (int)reader["id"],
            Login = (string)reader["login"],
            Password = (string)reader["password"],
            Role = (string)reader["role"],
            ClientId = reader["clientid"] != DBNull.Value? (int)reader["clientid"] : null,
            CompanyId = reader["companyid"] != DBNull.Value? (int)reader["companyid"] : null,
            CompanyEmployeeId = reader["companyemployeeid"] != DBNull.Value? (int)reader["companyemployeeid"] : null,
        };
    }
}