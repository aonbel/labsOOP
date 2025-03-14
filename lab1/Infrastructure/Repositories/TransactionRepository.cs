using Domain.Dtos;
using Domain.Interfaces.IRepositories;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Infrastructure.Repositories;

public class TransactionRepository(IOptions<PostgresOptions> options) : ITransactionRepository
{
    private readonly string _connectionString = options.Value.GetConnectionString();

    public async Task<int> AddAsync(TransactionDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                INSERT INTO transaction_records  
                                (recipientbankrecordid, receiverbankrecordid, iscancelled, date, amount)
                                VALUES
                                (@recipientbankrecordid, @receiverbankrecordid, @iscancelled, @date, @amount)
                                RETURNING id
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
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
        
        const string sqlQuery = "SELECT * FROM transaction_records WHERE id = @id";
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        await reader.ReadAsync(cancellationToken);

        return new TransactionDto
        {
            Id = (int)reader["id"],
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
        
        const string sqlQuery = "SELECT * FROM transaction_records";
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        var transactionDtos = new List<TransactionDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            transactionDtos.Add(new TransactionDto
            {
                Id = (int)reader["id"],
                RecipientBankRecordId = (int)reader["recipientbankrecordid"],
                ReceiverBankRecordId = (int)reader["receiverbankrecordid"],
                IsCancelled = (bool)reader["iscancelled"],
                Date = (DateTime)reader["date"],
                Amount = (decimal)reader["amount"]
            });
        }
        
        return transactionDtos;
    }

    public async Task UpdateAsync(TransactionDto entity, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                    UPDATE transaction_records SET
                                                                   name = @name,
                                                                   recipientbankrecordid = @recipientbankrecordid,
                                                                   receiverbankrecordid = @receiverbankrecordid,
                                                                   iscancelled = @iscancelled,
                                                                   date = @date,
                                                                   amount = @amount
                                    WHERE id = @id
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@id", entity.Id);
        command.Parameters.AddWithValue("@recipientbankrecordid", entity.RecipientBankRecordId);
        command.Parameters.AddWithValue("@receiverbankrecordid", entity.ReceiverBankRecordId);
        command.Parameters.AddWithValue("@iscancelled", entity.IsCancelled);
        command.Parameters.AddWithValue("@date", entity.Date);
        command.Parameters.AddWithValue("@amount", entity.Amount);
        
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sqlQuery = """
                                DELETE FROM transaction_records WHERE id = @id
                                """;
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
    
    public async Task<ICollection<TransactionDto>> GetTransactionsAsyncByRecipientBankRecordId(int bankRecordId,
        CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        
        const string sqlQuery = "SELECT * FROM transaction_records WHERE recipientbankrecordid = @recipientbankrecordid";
        
        var command = new NpgsqlCommand(sqlQuery, connection);
        
        command.Parameters.AddWithValue("@recipientbankrecordid", bankRecordId);
        
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        
        var transactionDtos = new List<TransactionDto>();

        while (await reader.ReadAsync(cancellationToken))
        {
            transactionDtos.Add(new TransactionDto
            {
                Id = (int)reader["id"],
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