using DataAccessExtensions.Extensions;
using DataAccessLayer.DTO;
using DataAccessLayer.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;

namespace Infrastructure.DataAccessLayer
{
    /// <summary>
    /// The chat repository class should be used for fetching and alterting
    /// data in the database.
    /// </summary>
    public class ChatRepository(string connectionString) : Repository(connectionString)
    {
        public ChatDTO GetChat(int chatId)
        {
            ChatDTO? chat = null;
            using (var transaction = CreateTransaction())
            {
                var command = transaction.CreateCommand(@"SELECT [Name], [IsGroupChat], [Hash] FROM [Chats]
                    WHERE ID = @ChatId
                ");

                command.Parameters.AddWithValue("@ChatId", chatId);

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return chat;
                    }

                    chat = new(reader["name"].ToString())
                    {
                        IsGroupChat = (bool)reader["IsGroupChat"],
                        Hash = reader["Hash"].ToString(),
                        Id = chatId
                    };
                }

                command = transaction.CreateCommand(@"SELECT UC.UserID, UC.ChatID, UC.IsAdmin, U.ID, UP.FirstName,
                    UP.LastName
                    FROM UserChats AS UC
                    INNER JOIN [Users] AS U
                    ON U.ID = UC.UserID
                    INNER JOIN [UserProfile] AS UP
                    ON UP.UserID = U.ID
                    WHERE UC.ChatID = @ChatId
                ");

                command.Parameters.AddWithValue("@ChatId", chatId);

                using (var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        chat.Users.Add(new()
                        {
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Id = (int)reader["UserID"],
                            IsAdmin = (bool)reader["IsAdmin"]
                        });
                    }
                }

                command = transaction.CreateCommand(@"
                    SELECT [ChatID], [Content], [UserID], [SendAt]
                    FROM [Messages]
                    WHERE [ChatID] = @ChatId
                ");

                command.Parameters.AddWithValue("@ChatId", chatId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        chat.Messages.Add(new(reader["Content"].ToString())
                        {
                           SendAt = (DateTime)reader["SendAt"],
                           User = chat.Users.FirstOrDefault(user=>user.Id == (int)reader["UserID"]) // more efficient then fetching all users again
                        });
                    }
                }
            }
            return chat;
        }

        public List<ChatDTO> GetChatsUserIsIn(int userId)
        {
            List<ChatDTO> userChats = new();
            using(var transaction = CreateTransaction())
            {
                var command = transaction.CreateCommand(@"SELECT TOP (1000) C.[ID]
                  ,C.[Name]
                  ,C.[IsGroupChat]
                  ,C.[Hash]
	              ,M.ID As LastReadMessageID
	              ,CASE WHEN M.ID IS NOT NULL THEN (SELECT TOP(1) Content FROM [Messages] WHERE ID > M.ID AND ChatID = C.ID) ELSE (SELECT TOP(1) Content FROM [Messages] WHERE ChatID = C.ID) END AS UnreadMessageContent
	              ,CASE WHEN M.ID IS NOT NULL THEN (SELECT COUNT(*) FROM [Messages] WHERE ID > M.ID AND ChatID = C.ID) ELSE (SELECT COUNT(*) FROM [Messages] WHERE ChatID = C.ID) END AS UnreadMessageCount
	             FROM [SeniorConnectPG8].[dbo].[Chats] AS C
	            LEFT JOIN [UserChats] AS UC
	            ON UC.ChatID = C.ID 
	            FULL OUTER JOIN [Messages] AS M
	            ON M.ID = UC.LastReadMessageID 
	            RIGHT JOIN [Users] AS U
	            ON U.ID = UC.UserID
	            WHERE U.ID = @UserId
                  ");

                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = command.ExecuteReader()) 
                {
                    while (reader.Read()) 
                    {
                        userChats.Add(new(reader["Name"].ToString())
                        {
                            Hash = reader["Hash"].ToString(),
                            IsGroupChat = (bool)reader["IsGroupChat"],
                            LastReadMessage = new MessageDTO(reader["UnreadMessageContent"].ToString()),
                            AmountOfUnreadMessages = (int)reader["UnreadMessageCount"]
                        });
                    }
                }
            }
            return userChats;
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