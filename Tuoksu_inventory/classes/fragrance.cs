using Azure.Core;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Data.SqlTypes;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace Tuoksu_inventory.classes
{
    public class fragrance
    {
        public int id { get; set; }
        public string name { get; set; }
        public string brand { get; set; }
        public string size { get; set; }
        public string notes { get; set; }
        public string category { get; set; }
        public string Mostcommonseason { get; set; }
        public string occasion { get; set; }
       

        public static async Task TestConnection()
        {
            // Implementation for testing database connection goes here
            string? connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
            connectionString = connectionString.Trim('"');
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Connection string is not set.");
            }
            else
            {
                Console.WriteLine("Connection string found.");
                connectionString = connectionString.Replace("\"", "").Trim();
                
            }

            CONNECTIONHELPER();





        }

        public static async Task  AddFragrance()
        {
            Console.WriteLine(" Adding a new fragrance...");
            
            await TestConnection();
            
        }
        public static async Task RemoveFragrance() 
        {
            Console.WriteLine(" Removing a fragrance by ID...");
            
            await TestConnection();

            
        }
        public static async Task ListFragrances()
        {
             Console.WriteLine(" Listing all fragrances...");
           
            await TestConnection();
            
        }
        public static void DebugConnectionString(string? input)
        {
            if (input == null) return;

            Console.WriteLine("--- String Debug Map ---");
            Console.WriteLine($"Total Length: {input.Length}");

            
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                
                string display = char.IsWhiteSpace(c) ? $"[{char.GetUnicodeCategory(c)}]" : c.ToString();

                
                if (i > 80 && i < 100)
                {
                    Console.WriteLine($"Index {i}: {display}");
                }
            }
            Console.WriteLine("------------------------");
        }
        public static bool userExists = false;
        public static async Task CheckIfUserExists(string username, SqlConnection sqlConnection)
        {

            var sqlcon = sqlConnection.ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
            string sqlQuery = "use fragrances; SELECT * FROM users WHERE username = @Username;";

            try
            {
                await sqlConnection.OpenAsync();

                users.Instance.username = username;
                Console.WriteLine("HELLO");
                
                
                var userList = (await sqlConnection.QueryAsync<users>(sqlQuery, new { Username = username })).ToList();
                foreach (var user in userList)
                {
                    if (user.username == username)
                    {
                        Console.WriteLine(" User found in database.");
                        userExists = true;
                    }
                    else
                    {
                        Console.WriteLine(" User not found in database.");
                        userExists = false;
                    }
                }
                
                sqlConnection.Close();

            }
            catch(Exception ex)
            {
                
                Console.WriteLine(" An error occurred 1: " + ex.Message);
                
            }
            
        }
        public static SqlConnection CONNECTIONHELPER()
        {
            string? connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
            connectionString = connectionString.Trim('"');
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Connection string is not set.");
                return null;
            }
            else
            {
                Console.WriteLine("Connection string found.");
                connectionString = connectionString.Replace("\"", "").Trim();
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            connection.OpenAsync();
                            Console.WriteLine("Database connection successful.");
                            return connection;


                        }
                        catch (SqlException ex)
                        {
                            Console.WriteLine("Database connection failed: " + ex.Message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("An error occurred: " + ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" An error occurred: " + ex.Message);
                }
            }
            return new SqlConnection(connectionString);
        }
        public static async Task CreateUser(string username, string password)
        {
            
            await CheckIfUserExists(username,CONNECTIONHELPER());
            if(userExists)
            {
                Console.WriteLine(" User already exists. Aborting user creation.");
                return;
            }
           // PasswordHasher.HashPassword(password, out byte[] salt);
           
            password = PasswordHasher.HashPassword(password, out byte[] salt);
            users.Instance.username = username;
            users.Instance.PasswordHash = password;
            users.Instance.salt = Convert.ToHexString(salt);

        }

        public static async Task CheckPasswordAsync(string password)
        {
    
            await TestConnection();
            
            



        }

    }
    public static class PasswordHasher
    {
        private const int saltSize = 64; // size of the hash
        private const int iterations = 350000; // number of iterations, Maybe a bit too much but idc.
        private static HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512; // hashing algorithm

        public static string HashPassword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(saltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                saltSize
            );  
            return Convert.ToHexString(hash);
        }
        public static bool VerifyPassword(string password, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                hashAlgorithm,
                saltSize);

            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
        }
    }
}
