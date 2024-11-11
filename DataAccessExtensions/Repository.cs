using Microsoft.Data.SqlClient;

namespace DataAccessExtensions.Extensions
{
    public abstract class Repository : IDisposable
    {
        private string _connectionString { get; init; }
        private SqlConnection _sqlConnection { get; init; }
        protected Repository(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            _connectionString = connectionString;
            _sqlConnection = new SqlConnection(_connectionString);
        }

        protected Transaction CreateTransaction()
        {
            return new Transaction(this);
        }

        public SqlCommand CreateCommand(string commandText)
        {
            var command = _sqlConnection.CreateCommand();
            command.CommandText = commandText;
            return command;
        }

        public SqlCommand CreateCommand(string commandText, SqlTransaction transaction)
        {
            var command = _sqlConnection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = commandText;
            return command;
        }

        public SqlTransaction CreateSqlTransaction()
        {
            return _sqlConnection.BeginTransaction();
        }

        public void OpenConnection()
        {
            _sqlConnection.Open();
        }

        public void CloseConnection()
        {
            _sqlConnection.Close();
        }
        /// <summary>
        /// Only use for testing purposes!
        /// </summary>
        public void ClearDatabase()
        {
            using(var transaction = CreateTransaction())
            {
                var command = transaction.CreateCommand(@"
                        EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';

                        EXEC sp_MSForEachTable 'DELETE FROM ?';

                        EXEC sp_MSForEachTable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';
                ");
                command.ExecuteNonQuery();
            }
        }

        private void Dispose()
        {
            _sqlConnection.Dispose();
        }

        void IDisposable.Dispose()
        {
            _sqlConnection.Dispose();
        }
    }
}