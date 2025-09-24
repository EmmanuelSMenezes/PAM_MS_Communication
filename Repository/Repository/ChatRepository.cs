using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Domain.Model;
using Npgsql;
using Serilog;

namespace Infrastructure.Repository
{
  public class ChatRepository : IChatRepository
  {
    private readonly string _connectionString;
    private readonly ILogger _logger;
    public ChatRepository(string connectionString, ILogger logger)
    {
      _connectionString = connectionString;
      _logger = logger;
    }

    public Chat CreateChat(CreateChatRequest createChatRequest)
    {
      try
      {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
          string sql = @$"INSERT INTO communication.chat (description, created_at, created_by";
          if (createChatRequest.Order_id != Guid.Empty && createChatRequest.Order_id != null)
          {
            sql += @$", order_id";
          }
          sql += @$")
            VALUES (
              '{createChatRequest.Description}'
              , CURRENT_TIMESTAMP
              , '{createChatRequest.Created_by}'";
          if (createChatRequest.Order_id != Guid.Empty && createChatRequest.Order_id != null)
          {
            sql += $@", '{createChatRequest.Order_id}'";
          }
          sql += $@") RETURNING *;";

          connection.Open();
          var transaction = connection.BeginTransaction();


          var response = connection.Query<Chat>(sql).FirstOrDefault<Chat>();

          List<Guid> Members = new List<Guid>();
          foreach (var member in createChatRequest.Members)
          {
            string sqlUserExists = $"SELECT user_id FROM authentication.user WHERE user_id = '{member}';";

            var memberExists = connection.Query<Guid>(sqlUserExists).FirstOrDefault();
            if (memberExists == Guid.Empty || memberExists == null) throw new Exception("oneOfTheUsersDoesNotExist");


            string sqlMember = @$"
              INSERT INTO communication.chat_member
              (chat_id, user_id)
              VALUES
              ('{response.Chat_id}', '{member}')
              RETURNING user_id;
            ";

            var memberId = connection.Query<Guid>(sqlMember).FirstOrDefault();
            Members.Add(memberId);
          }
          response.Members = Members.ToArray<Guid>();
          transaction.Commit();
          connection.Close();
          string concatenatedMembers = string.Join("','", response.Members.Select(x => x.ToString()).ToArray());
          string sqlProfile = @$"SELECT *, p.fullname as name FROM authentication.user u INNER JOIN authentication.profile p ON p.user_id = u.user_id WHERE u.user_id IN ('{concatenatedMembers}');";
          var membersProfile = connection.Query<Member>(sqlProfile).ToList();
          response.MembersProfile = membersProfile;
          _logger.Information("[ChatRepository - CreateChat]: Chat created successfully.");
          return response;
        }
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[ChatRepository - CreateChat]: Error while create chat on database.");
        throw ex;
      }
    }

    public Chat GetChatById(Guid chat_id)
    {
      try
      {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
          string sql = @$"SELECT * FROM communication.chat WHERE chat_id = '{chat_id}';";
          var response = connection.Query<Chat>(sql).FirstOrDefault<Chat>();

          string sqlMembers = @$"SELECT user_id FROM communication.chat_member WHERE chat_id = '{response.Chat_id}'";
          var responseMembers = connection.Query<Guid>(sqlMembers).ToList();

          response.Members = responseMembers.ToArray();

          string concatenatedMembers = string.Join("','", response.Members.Select(x => x.ToString()).ToArray());
          string sqlProfile = @$"SELECT *, p.fullname as name FROM authentication.user u INNER JOIN authentication.profile p ON p.user_id = u.user_id WHERE u.user_id IN ('{concatenatedMembers}');";
          var membersProfile = connection.Query<Member>(sqlProfile).ToList();
          response.MembersProfile = membersProfile;
          _logger.Information("[ChatRepository - CreateChat]: Chat created successfully.");
          return response;
        }
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[ChatRepository - CreateChat]: Error while create chat on database.");
        throw ex;
      }
    }

    public List<Chat> ListChatsWithOneMemberId(Guid member_id)
    {
      try
      {
        string sql = @$"
          SELECT c.*,
          (select array_agg(user_id) from communication.chat_member cm2 where chat_id = c.chat_id and user_id IS NOT NULL) as Members
          FROM communication.chat c
          INNER join communication.chat_member cm
          on c.chat_id = cm.chat_id
          where cm.user_id = '{member_id}';
        ";
        using (var connection = new NpgsqlConnection(_connectionString))
        {
          var response = connection.Query<Chat>(sql).ToList();
          foreach (var chat in response)
          {
            var lastMessageSql = @$"SELECT m.*, mt.tag as MessageType FROM communication.message m INNER JOIN communication.message_type mt ON m.message_type = mt.message_type_id WHERE m.chat_id = '{chat.Chat_id}' ORDER BY m.created_at DESC;";
            var lastMessageResponse = connection.Query<Message>(lastMessageSql).FirstOrDefault();
            chat.LastMessage = lastMessageResponse;

            var countMessageSql = @$"SELECT count(m.message_id) FROM communication.message m WHERE m.chat_id = '{chat.Chat_id}' and m.read_at IS NULL and m.sender_id != '{member_id}';";
            var countMessageResponse = connection.Query<int>(countMessageSql).FirstOrDefault();
            chat.UnReadCountMessages = countMessageResponse;

            var messagesSql = @$"
                SELECT m.*, mt.tag as MessageType FROM communication.message m
                INNER JOIN communication.message_type mt
                ON m.message_type = mt.message_type_id
                WHERE chat_id = '{chat.Chat_id}'
                ORDER BY m.created_at ASC;
              ";
            var messagesResponse = connection.Query<Message>(messagesSql).ToList();
            chat.Messages = messagesResponse;

            foreach (var member in chat.Members)
            {
              var sqlMember = @$"SELECT u.user_id, p.avatar, p.fullname as name FROM authentication.user u INNER JOIN authentication.profile p ON u.user_id = p.user_id WHERE u.user_id = '{member}'";
              var responseMember = connection.Query<Member>(sqlMember).FirstOrDefault();
              chat.MembersProfile.Add(responseMember);
            }
          }
          _logger.Information("[ChatRepository - CreateChat]: Chat created successfully.");
          return response;
        }
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[ChatRepository - CreateChat]: Error while create chat on database.");
        throw ex;
      }
    }

    public List<Guid> ListUserIdsAvaliable()
    {
      try
      {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
          string sql = @$"
            SELECT user_id FROM authentication.user;
          ";

          var response = connection.Query<Guid>(sql).ToList();
          return response;
        }
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[ChatRepository - ListUserIdsAvaliable]: Error while list users avaliable for chat on database.");
        throw ex;
      }
    }

    public Chat UpdateChat(UpdateChatRequest updateChatRequest)
    {
      try
      {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
          string sql = @$"
            UPDATE communication.chat
            SET closed=CURRENT_TIMESTAMP, closed_by='{updateChatRequest.Closed_by}'
            WHERE chat_id='{updateChatRequest.Chat_id}' RETURNING *;
          ";

          var response = connection.Query<Chat>(sql).FirstOrDefault();
          return response;
        }
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "[ChatRepository - UpdateChat]: Error while update chat in database.");
        throw ex;
      }
    }
  }
}
