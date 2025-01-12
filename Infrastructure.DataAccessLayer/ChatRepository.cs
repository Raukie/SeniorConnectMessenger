using DataAccessExtensions.Extensions;
using DataAccessLayer.DTO;
using DataAccessLayer.Extensions;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Data.Common;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Transactions;

namespace Infrastructure.DataAccessLayer
{
    /// <summary>
    /// The chat repository class should be used for fetching and alterting
    /// data in the database.
    /// </summary>
    public class ChatRepository(string connectionString) : Repository(connectionString), IChatStorage
    {
        /// <summary>
        /// Updates the last read message id and sends the difference in messages
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public List<MessageDTO> UpdateLastReadMessage(int chatId, int userId)
        {
            var chat = GetChat(chatId, userId, false);
            List<MessageDTO> unreadMessages = new();

            using (var transaction = CreateTransaction())
            {
                var command = transaction.CreateCommand(@"
                    SELECT [LastReadMessageID] FROM [UserChats] WHERE [UserID] = @UserId AND [ChatID] = @ChatId
                ");

                command.Parameters.AddWithValue("@ChatId", chatId);
                command.Parameters.AddWithValue("@UserId", userId);

                // could be null if chat has been removed after the request was sent out to check for new messages
                var lastReadMessage = command.ExecuteScalar();

                if(lastReadMessage == null)
                {
                    return new();
                }

                int lastReadMessageID = (int)lastReadMessage;

                command = transaction.CreateCommand(@"
                    SELECT [ID], [ChatID], [Content], [UserID], [SendAt]
                    FROM [Messages]
                    WHERE [ChatID] = @ChatId AND [ID] > @LastReadMessageID
                    ORDER BY [SendAt] ASC
                ");

                command.Parameters.AddWithValue("@LastReadMessageID", lastReadMessageID);
                command.Parameters.AddWithValue("@ChatId", chatId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        unreadMessages.Add(new(reader["Content"].ToString())
                        {
                            SendAt = (DateTime)reader["SendAt"],
                            Id = (int)reader["ID"],
                            User = (reader["UserID"] != DBNull.Value) ? chat.Users.FirstOrDefault(user => user.Id == (int)reader["UserID"]) : null // more efficient then fetching all users again
                        });
                        if ((int)reader["ID"] > lastReadMessageID)
                        {
                            lastReadMessageID = (int)reader["ID"];
                        }
                    }
                }

                command = transaction.CreateCommand(@"UPDATE [UserChats] SET LastReadMessageID = @LastReadMessageId WHERE [ChatID] = @ChatId AND [UserID] = @UserId");
                command.Parameters.AddWithValue("@LastReadMessageID", lastReadMessageID);
                command.Parameters.AddWithValue("@ChatId", chatId);
                command.Parameters.AddWithValue("@UserId", userId);
                command.ExecuteNonQuery();
            }
            return unreadMessages;
        }

        public ChatDTO GetChat(int chatId, int userId, bool updateLastReadMessage = false)
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
                    while (reader.Read())
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
                    SELECT [ChatID], [Content], [UserID], [SendAt], [ID]
                    FROM [Messages]
                    WHERE [ChatID] = @ChatId
                    ORDER BY [SendAt] DESC
                ");

                command.Parameters.AddWithValue("@ChatId", chatId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        chat.Messages.Add(new(reader["Content"].ToString())
                        {
                            SendAt = (DateTime)reader["SendAt"],
                            Id = (int)reader["ID"],
                            User = (reader["UserID"] != DBNull.Value) ? chat.Users.FirstOrDefault(user => user.Id == (int)reader["UserID"]) : null // more efficient then fetching all users again
                        });
                    }
                }

                if (updateLastReadMessage && chat.Messages.Count > 0)
                {
                    command = transaction.CreateCommand(@"UPDATE [UserChats] SET LastReadMessageID = @LastReadMessageId WHERE [ChatID] = @ChatId AND [UserID] = @UserId");
                    command.Parameters.AddWithValue("@LastReadMessageID", chat.Messages.OrderByDescending(c => c.Id).First().Id);
                    command.Parameters.AddWithValue("@ChatId", chatId);
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.ExecuteNonQuery();

                    chat.LastReadMessage = chat.Messages.OrderByDescending(chat => chat.Id).First();
                    chat.AmountOfUnreadMessages = 0;
                }
            }
            return chat;
        }

        public bool IsUserAdmin(int chatId, int userId)
        {
            using (var transaction = CreateTransaction())
            {
                var command = transaction.CreateCommand(@"
                    SELECT [IsAdmin] FROM [UserChats] WHERE [UserID] = @UserID AND [ChatID] = @ChatId
                ");

                command.Parameters.AddWithValue("@ChatId", chatId);
                command.Parameters.AddWithValue("@UserId", userId);

                return (bool)command.ExecuteScalar(); // if more then one row altered it is a success
            }
        }


        public bool MakeUserAdmin(int chatId, int userId)
        {
            using (var transaction = CreateTransaction())
            {
                var command = transaction.CreateCommand(@"
                    UPDATE [UserChats] SET [IsAdmin] = 1 WHERE [UserID] = @UserID AND [ChatID] = @ChatId
                ");

                command.Parameters.AddWithValue("@ChatId", chatId);
                command.Parameters.AddWithValue("@UserId", userId);

                return command.ExecuteNonQuery() > 0; // if more then one row altered it is a success
            }
        }

        public bool RemoveUserFromChat(int chatId, int userId)
        {
            using (var transaction = CreateTransaction())
            {
                var command = transaction.CreateCommand(@"
                    DELETE FROM [UserChats] WHERE [UserID] = @UserID AND [ChatID] = @ChatId
                ");

                command.Parameters.AddWithValue("@ChatId", chatId);
                command.Parameters.AddWithValue("@UserId", userId);

                return command.ExecuteNonQuery() > 0; // if more then one row altered it is a success
            }
        }

        public List<ChatDTO> GetChatsUserIsIn(int userId)
        {
            List<ChatDTO> userChats = new();
            using (var transaction = CreateTransaction())
            {
                var command = transaction.CreateCommand(@"SELECT C.[ID]
                  ,C.[Name]
                  ,C.[IsGroupChat]
                  ,C.[Hash]
	              ,M.ID As LastReadMessageID
	              ,(SELECT TOP(1) ID FROM [Messages] WHERE C.ID = ChatID ORDER BY [SendAt] DESC ) AS LastFetchedMessageID
	              ,(SELECT TOP(1) Content FROM [Messages] WHERE C.ID = ChatID ORDER BY [SendAt] DESC ) AS UnreadMessageContent
	              ,CASE WHEN M.ID IS NOT NULL THEN (SELECT COUNT(*) FROM [Messages] WHERE ID > M.ID AND ChatID = C.ID) ELSE (SELECT COUNT(*) FROM [Messages] WHERE ChatID = C.ID) END AS UnreadMessageCount
	             FROM [dbo].[Chats] AS C
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
                            LastReadMessage = new MessageDTO(reader["UnreadMessageContent"].ToString()) { Id = (int)reader["LastFetchedMessageID"] },
                            AmountOfUnreadMessages = (int)reader["UnreadMessageCount"],
                            Id = (int)reader["ID"]
                        });
                    }
                }
            }
            return userChats;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatDto"></param>
        /// <param name="userCreatedBy"></param>
        /// <param name="overrideDate"> Only for the database seeder!!!</param>
        /// <returns></returns>
        public ChatDTO CreateChat(ChatDTO chatDto, UserDTO userCreatedBy, DateTime? overrideDate = null)
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
                command.Parameters.Add(new SqlParameter($"@LastReadMessageID", SqlDbType.Int) { Value = DBNull.Value });
                command.CommandText = command.CommandText.TrimEnd('\n').TrimEnd(' ').TrimEnd(',');
                command.ExecuteNonQuery();
            }

                CreateMessage(chatDto.Id, new MessageDTO($"{userCreatedBy.FirstName} heeft deze chat aangemaakt", null, overrideDate));
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

        public bool AddUserToChat(int chatId, UserDTO userToAdd)
        {
            using (var transaction = CreateTransaction())
            {
                var command = transaction.CreateCommand(@"
                    INSERT INTO [UserChats] (UserID, LastReadMessageID, ChatID, IsAdmin)
                    VALUES (@UserID, @LastReadMessageID, @ChatID, @IsAdmin)
                ");
                command.Parameters.AddWithValue("@UserID", userToAdd.Id);
                command.Parameters.AddWithValue("@ChatID", chatId);
                command.Parameters.AddWithValue("@IsAdmin", false); 
                command.Parameters.AddWithValue("@LastReadMessageID", DBNull.Value);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected <= 0)
                {
                    return false; // If no rows were affected, the insert failed
                }

                var welcomeMessage = new MessageDTO($"{userToAdd.FirstName} {userToAdd.LastName} neemt deel aan de groep")
                {
                };

                CreateMessage(chatId, welcomeMessage);
            }

            return true; // Successfully added the user to the chat
        }

    }
}