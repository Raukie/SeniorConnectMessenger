using DataAccessExtensions.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.DataAccessLayer
{
    /// <summary>
    /// The chat repository class should be used for fetching and alterting
    /// data in the database.
    /// </summary>
    public class ChatRepository(string connectionString) : Repository(connectionString)
    {
        public GetAllChats()
        {
            using(var transaction = CreateTransaction())
            {
                var command = transaction.CreateCommand("SELECT * ");
            }
            return new();
        }

        public ChatDTO CreateChat(ChatDTO chatDto, UserDTO userCreatedBy)
        {
            using (var transaction = CreateTransaction())
            {
                var command = transaction.CreateCommand(@"INSERT INTO [Chats]
                    ([Name], [IsGroupChat], [Hash]) output INSERTED.ID VALUES (@name, @isGroupChat, @hash)
                ");
                command.Parameters.AddWithValue("@name", chatDto.Name);
                command.Parameters.AddWithValue("@hash", chatDto.CalculateHash());
                command.Parameters.AddWithValue("@isGroupChat", chatDto.IsGroupChat);
                chatDto.Id = (int)command.ExecuteScalar();

                command = transaction.CreateCommand("INSERT INTO [UserChats] (UserID, LastReadMessageID, ChatID, IsAdmin) VALUES \n");
                int commandCount = 1;


                foreach (var user in chatDto.Users)
                {
                    commandCount++;
                    command.CommandText += $"(@UserID{commandCount}, @LastReadMessageID, @ChatID, @IsAdmin{commandCount}), \n";
                    command.Parameters.AddWithValue($"@UserID{commandCount}", user.Id);
                    command.Parameters.AddWithValue($"@IsAdmin{commandCount}", user.IsAdmin);
                }
                command.Parameters.AddWithValue($"@ChatID", chatDto.Id);
                command.Parameters.Add(new SqlParameter($"@LastReadMessageID", SqlDbType.Int) { Value = DBNull.Value});
                command.CommandText = command.CommandText.TrimEnd('\n').TrimEnd(' ').TrimEnd(',');
                command.ExecuteNonQuery();
            }

            CreateMessage(chatDto.Id, new MessageDTO($"{userCreatedBy.FirstName} heeft deze chat aangemaakt", null));
            return chatDto;
        }

        public void CreateMessage(int chatId, MessageDTO message)
        {
            using (var transaction = CreateTransaction())
            {
                var command = transaction.CreateCommand(@"INSERT INTO [Messages]
                    ([ChatID], [Content], [UserID], [SendAt]) VALUES (@ChatId, @Content, @UserID, @SendAt)
                ");

                command.Parameters.AddWithValue("@ChatID", chatId);
                command.Parameters.AddWithValue("@Content", message.Content);
                command.Parameters.AddWithValue("@UserID", (message.User == null) ? DBNull.Value : message.User.Id);
                command.Parameters.AddWithValue("@SendAt", message.SendAt);

                command.ExecuteNonQuery();
            }
        }
    }
}