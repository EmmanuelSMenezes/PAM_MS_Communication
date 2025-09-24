using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Domain.Model;
using Npgsql;
using Serilog;

namespace Infrastructure.Repository
{
  public class MessageRepository : IMessageRepository
  {
    private readonly string _connectionString;
    private readonly ILogger _logger;
    public MessageRepository(string connectionString, ILogger logger)
    {
      _connectionString = connectionString;
      _logger = logger;
    }

    public Message CreateMessage(CreateMessageRequest createMessageRequest)
    {
      try
      {
        var sql = @$"INSERT INTO communication.message
          (chat_id, created_by, sender_id, message_type, read_at, created_at, content)
          VALUES (
              '{createMessageRequest.Chat_id}'
            , '{createMessageRequest.Created_by}'
            , '{createMessageRequest.Sender_id}'
            , '{createMessageRequest.MessageType}'
            , NULL
            , '{createMessageRequest.Created_at.ToString("o")}'
            , '{createMessageRequest.Content}')
            RETURNING *;
          ";
        using (var connection = new NpgsqlConnection(_connectionString))
        {
          var response = connection.Query<Message>(sql).FirstOrDefault();
          _logger.Information("[MessageRepository - CreateMessage]: Message successfully created on db.");
          return response;
        }
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[MessageRepository - CreateMessage]: Error while create message on db.");
        throw ex;
      }
    }

    public MessageType GetMessageTypeByTag(string tag)
    {
      try
      {
        var sql = @$"SELECT * FROM communication.message_type WHERE tag = '{tag}'";

        using (var connection = new NpgsqlConnection(_connectionString))
        {
          var response = connection.Query<MessageType>(sql).FirstOrDefault();
          return response;
        }
      }
      catch (Exception ex)
      {

        throw ex;
      }
    }

    public List<Message> ListMessagesByChatId(Guid chat_id)
    {
      try
      {
        var sql = @$"SELECT * FROM communication.message WHERE chat_id = '{chat_id}';";
        using (var connection = new NpgsqlConnection(_connectionString))
        {
          var response = connection.Query<Message>(sql).ToList();
          _logger.Information("[MessageRepository - CreateMessage]: Messages successfully retrieved on db.");
          return response;
        }
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[MessageRepository - CreateMessage]: Error while retrieve messages on db.");
        throw ex;
      }
    }

    public Message UpdateMessage(UpdateMessageRequest updateMessageRequest)
    {
      try
      {
        var sql = @$"
            UPDATE communication.message
            SET read_at='{updateMessageRequest.Read_at.ToString("o")}'
            WHERE message_id = '{updateMessageRequest.Message_id}' and chat_id = '{updateMessageRequest.Chat_id}'
            RETURNING *;
        ";
        using (var connection = new NpgsqlConnection(_connectionString))
        {
          var response = connection.Query<Message>(sql).FirstOrDefault();
          _logger.Information("[MessageRepository - CreateMessage]: Message successfully updated on db.");
          return response;
        }
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[MessageRepository - CreateMessage]: Error while update message on db.");
        throw ex;
      }
    }
  }
}
