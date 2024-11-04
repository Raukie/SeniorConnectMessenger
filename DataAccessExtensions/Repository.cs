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
            return _sqlConnection.CreateCommand();
        }

        public SqlCommand CreateCommand(string commandText, SqlTransaction transaction)
        {
            return _sqlConnection.CreateCommand();
        }

        public SqlTransaction CreateSqlTransaction(string name)
        {
            return _sqlConnection.BeginTransaction(name);
        }

        public void OpenConnection()
        {
            _sqlConnection.Open();
        }

        public void CloseConnection()
        {
            _sqlConnection.Close();
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