using DataAccessLayer;
using Infrastructure.DataAccessLayer;
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=SeniorConnectPG8;Integrated Security=True;Trust Server Certificate=True";

var chatRepository = new ChatRepository(connectionString);
var userRepository = new UserRepository(connectionString);

var databaseSeeder = new DatabaseSeeder(chatRepository, userRepository, 50, 100, 200);
databaseSeeder.SeedDatabase();