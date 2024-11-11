using DataAccessLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Runtime.Serialization.Formatters.Binary;

namespace DataAccessLayer.Extensions
{
    public static class ChatExtensions
    {
        public static string CalculateHash(this ChatDTO chatDto)
        {
            string users = string.Join(null, chatDto.Users.OrderBy(user=>user.Username).Select(user=>user.Username));
            string hashString = users + chatDto.Name;
            return Convert.ToHexString(SHA1.HashData(Encoding.UTF8.GetBytes(hashString)));
        }

    }
}
