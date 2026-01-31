using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuoksu_inventory.Interfaces;
using Dapper;

namespace Tuoksu_inventory.classes
{
    public class SuggestionLogic : IFragranceRepo
    {
        private readonly string _connectionString;
        public SuggestionLogic(string connString) => _connectionString = connString;

        public async Task<IEnumerable<fragrance>> GetAllAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            return await conn.QueryAsync<fragrance>("SELECT * FROM Fragrances");
        }
    }
}
