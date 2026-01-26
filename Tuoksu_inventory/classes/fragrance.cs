using Dapper;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
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

            





        }

        public static async Task AddFragrance()
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

                var userList = (await sqlConnection.QueryAsync<users>(sqlQuery, new { Username = username })).ToList();
                foreach (var user in userList)
                {
                    if (user.username == username)
                    {                      
                        userExists = true;
                    }
                    else
                    {                      
                        userExists = false;
                    }
                }

                sqlConnection.Close();

            }
            catch (Exception ex)
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
                return null;
            }
            else
            {
                
                connectionString = connectionString.Replace("\"", "").Trim();
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        try
                        {
                            connection.OpenAsync();
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
        public static async Task CreateUser(string username, string password,string email)
        {

            await CheckIfUserExists(username, CONNECTIONHELPER());
            if (userExists)
            {
                Console.WriteLine(" User already exists. Aborting user creation.");
                return;
            }
            else
            {
                Console.WriteLine(" Creating new user...");
                var sqlConnection = CONNECTIONHELPER();

                var sqlcon = sqlConnection.ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

                string sqlQuery = "INSERT INTO users (username, PasswordHash, salt, email) VALUES (@Username, @PasswordHash, @Salt, @Email);";

                byte[] saltBytes;
                string passwordHash = PasswordHasher.HashPassword(password, out saltBytes);
                string saltHex = Convert.ToHexString(saltBytes);

                users.Instance.email = email;


                users.Instance.username = username;
                users.Instance.PasswordHash = passwordHash;
                users.Instance.salt = saltHex;



                try
                {
                    var result = await sqlConnection.ExecuteAsync(sqlQuery, new
                    {
                      Email= email,
                      Username =  username,
                      PasswordHash = passwordHash,
                      Salt = saltHex,
                    });

                    Console.WriteLine(" User created successfully.");
                }
                catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601) // Unique constraint error number
                {
                    Console.WriteLine(" User already exists. Aborting user creation.");

                }
                catch (Exception ex)
                {
                    Console.WriteLine(" An error occurred while creating the user: " + ex.Message);
                }
            }   

        }
        public static async Task VerifyEmail(string email, SqlConnection sql)
        {

            var sqlcon = sql.ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
            string sqlQuery = "SELECT * FROM users WHERE email = @Email;";
            try
            {
                await sql.ExecuteAsync(sqlQuery, new { Email = email });
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(" An error occurred 1: " + ex.Message);
            }
            finally
            {
                await sql.CloseAsync();

            }
        }
        public static async Task<int> SetId(string username,SqlConnection sql)
        {
           string sqlQuery = "use fragrances ;select top 1 * from users order by id desc;";
            try
            {
                await sql.OpenAsync();
                var userList = (await sql.QueryAsync<users>(sqlQuery)).ToList();
                foreach (var user in userList)
                {
                    if (user.username == username)
                    {
                        users.Instance.id = user.id ++;
                        return users.Instance.id;
                    }
                }
                sql.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(" An error occurred 1: " + ex.Message);
            }
            finally
            {
                await sql.CloseAsync();
            }

                return 0;
        }
        public static async Task CheckPasswordAsync(string password)
        {

          





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
