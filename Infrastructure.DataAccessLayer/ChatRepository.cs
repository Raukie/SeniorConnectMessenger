using DataAccessExtensions.Extensions;
using Microsoft.Data.SqlClient;

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
                var t = transaction.CreateCommand("SELECT * FROM ");

            }
        }
    }
}