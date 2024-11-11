using Microsoft.Data.SqlClient;

namespace DataAccessExtensions.Extensions
{
    public class Transaction : IDisposable
    {
        private List<SqlCommand> _commands { get; init; } = new List<SqlCommand>();
        private Repository _repository { get; init; }
        private SqlTransaction _sqlTransaction { get; init; }
        public Transaction(Repository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            repository.OpenConnection();
            _sqlTransaction = repository.CreateSqlTransaction();
        }

        public SqlCommand CreateCommand(string commandText)
        {
            return _repository.CreateCommand(commandText, _sqlTransaction);
        }

        public void Dispose()
        {
            if(_repository == null)
            {
                throw new ArgumentNullException(nameof(_repository)); // #TODO Is this the correct exception type?
            } 
            _sqlTransaction.Commit();
            _repository.CloseConnection();
            foreach (var command in _commands)
            {
                command.Dispose();
            }
        }
    }
}