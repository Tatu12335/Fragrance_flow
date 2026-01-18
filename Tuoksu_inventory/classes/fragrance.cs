using Azure.Core;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuoksu_inventory.classes
{
    public class fragrance
    {
        public int id;
        public string name;
        public string brand;
        public string size;
        public string notes;
        public string category;
        public string Mostcommonseason;
        public string occasion;

        public static Task TestConnection()
        {
            // Implementation for testing database connection goes here
            string? connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
            if(string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Connection string is not set.");
            }
            else
            {
                Console.WriteLine("Connection string found.");
            }

            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Database connection successful.");
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

            return Task.CompletedTask;



        }

        public static Task  AddFragrance()
        {
            Console.WriteLine(" Adding a new fragrance...");
            // Implementation for adding a fragrance goes here
            TestConnection();
            return Task.CompletedTask;
        }
        public static Task RemoveFragrance() 
        {
            Console.WriteLine(" Removing a fragrance by ID...");
            // Implementation for removing a fragrance goes here
            TestConnection();

            return Task.CompletedTask;
        }
        public static Task ListFragrances()
        {
             Console.WriteLine(" Listing all fragrances...");
            // Implementation for listing fragrances goes here
            return Task.CompletedTask;
        }

    }
}
