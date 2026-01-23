using Azure.Core;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
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

            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Database connection successful.");
                    connection.Close(); 

                    //SqlCommand command = new SqlCommand("use fragrances; SELECT * FROM collection", connection);
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("Database connection failed: " + ex.Message);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            



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
        public static async Task CheckPasswordAsync(string username)
        {
            Console.WriteLine(" Checking user...");
            await TestConnection();


            
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
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            Console.WriteLine("Database connection successful.");
                            

                            //SqlCommand command = new SqlCommand("use fragrances; SELECT * FROM collection", connection);
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
                catch(Exception ex)
                {
                    Console.WriteLine(" An error occurred: " + ex.Message);
                }
            }



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
