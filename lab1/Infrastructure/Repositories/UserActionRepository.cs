using Domain.Dtos;
using Domain.Entities;
using Domain.Interfaces.IRepositories;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories;

public class UserActionRepository(IOptions<PostgresOptions> options)
    : IUserActionRepository
{
    private readonly string _connectionString = options.Value.GetConnectionString();

    public async Task<int> AddAsync(UserActionDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken);

        const string qslQuery = """
                                INSERT INTO action_records
                                    (name, userid, date, actiontargettype, previousstateid, actiontype) 
                                    VALUES 
                                    (@name, @userid, @date, @actiontargettype, @previousstateid, @actiontype)
                                    RETURNING id
                                """;

        var command = new NpgsqlCommand(qslQuery, connection);
        
        command.Parameters.AddWithValue("@name", entity.Name);
        command.Parameters.AddWithValue("@userid", entity.UserId);
        command.Parameters.AddWithValue("@date", entity.Date);
        command.Parameters.AddWithValue("@actiontargettype", entity.ActionTargetType);
        command.Parameters.AddWithValue("@previousstateid", entity.PreviousStateId);
        command.Parameters.AddWithValue("@actiontype", entity.Type);

        return (int)(await command.ExecuteScalarAsync(cancellationToken) ?? throw new NpgsqlException());
    }

    public async Task<UserActionDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM action_records 
                                         WHERE id=@id
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@id", id);
        
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        await reader.ReadAsync(cancellationToken);

        return new UserActionDto
        {
            Id = (int)reader["id"],
            Name = (string)reader["name"],
            UserId = (int)reader["userid"],
            Date = (DateTime)reader["date"],
            ActionTargetType = (string)reader["actiontargettype"],
            PreviousStateId = (int)reader["previousstateid"],
            Type = (int)reader["actiontype"]
        };
    }

    public async Task<ICollection<UserActionDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM action_records
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        var actionDtos = new List<UserActionDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            actionDtos.Add(new UserActionDto
            {
                Id = (int)reader["id"],
                Name = (string)reader["name"],
                UserId = (int)reader["userid"],
                Date = (DateTime)reader["date"],
                ActionTargetType = (string)reader["actiontargettype"],
                PreviousStateId = (int)reader["previousstateid"],
                Type = (int)reader["actiontype"]
            });
        }

        return actionDtos;
    }
    
    public async Task<ICollection<UserActionDto>> GetAllActionsByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM action_records 
                                         WHERE userid=@userid
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@userid", userId);
        
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        var actionDtos = new List<UserActionDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            actionDtos.Add(new UserActionDto
            {
                Id = (int)reader["id"],
                Name = (string)reader["name"],
                UserId = (int)reader["userid"],
                Date = (DateTime)reader["date"],
                ActionTargetType = (string)reader["actiontargettype"],
                PreviousStateId = (int)reader["previousstateid"],
                Type = (int)reader["actiontype"]
            });
        }

        return actionDtos;
    }
}