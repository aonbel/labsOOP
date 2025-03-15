using Domain.Dtos;
using Domain.Interfaces.IRepositories;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories.StateRepositories;

public class TransactionStateRepository(IOptions<PostgresOptions> options) : ICRRepository<TransactionDto>
{
    private readonly string _connectionString = options.Value.GetConnectionString();

    public async Task<int> AddAsync(TransactionDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                INSERT INTO transaction_state_records  
                                (transactionid, recipientbankrecordid, receiverbankrecordid, iscancelled, date, amount)
                                VALUES
                                (@transactionid, @recipientbankrecordid, @receiverbankrecordid, @iscancelled, @date, @amount)
                                RETURNING id
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@transactionid", entity.Id);
        command.Parameters.AddWithValue("@recipientbankrecordid", entity.RecipientBankRecordId);
        command.Parameters.AddWithValue("@receiverbankrecordid", entity.ReceiverBankRecordId);
        command.Parameters.AddWithValue("@iscancelled", entity.IsCancelled);
        command.Parameters.AddWithValue("@date", entity.Date);
        command.Parameters.AddWithValue("@amount", entity.Amount);
        
        return (int)(await command.ExecuteScalarAsync(cancellationToken) ?? throw new NpgsqlException());
    }

    public async Task<TransactionDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                SELECT * FROM transaction_state_records
                                         WHERE id = @id
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@id", id);
        
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        await reader.ReadAsync(cancellationToken);

        return new TransactionDto
        {
            Id = (int)reader["transactionid"],
            RecipientBankRecordId = (int)reader["recipientbankrecordid"],
            ReceiverBankRecordId = (int)reader["receiverbankrecordid"],
            IsCancelled = (bool)reader["iscancelled"],
            Date = (DateTime)reader["date"],
            Amount = (decimal)reader["amount"]
        };
    }

    public async Task<ICollection<TransactionDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        
        const string sqlQuery = "SELECT * FROM transaction_state_records";
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        var transactionDtos = new List<TransactionDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            transactionDtos.Add(new TransactionDto
            {
                Id = (int)reader["transactionid"],
                RecipientBankRecordId = (int)reader["recipientbankrecordid"],
                ReceiverBankRecordId = (int)reader["receiverbankrecordid"],
                IsCancelled = (bool)reader["iscancelled"],
                Date = (DateTime)reader["date"],
                Amount = (decimal)reader["amount"]
            });
        }
        
        return transactionDtos;
    }
}