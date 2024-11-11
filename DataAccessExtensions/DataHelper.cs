using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessExtensions
{
    public static class DataHelper
    {
        public static string HashPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }

        public static bool Verify(string password, string plainPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, plainPassword);
        }
    }
}
