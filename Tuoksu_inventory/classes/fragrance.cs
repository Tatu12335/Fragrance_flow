using Dapper;
using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
namespace Tuoksu_inventory.classes
{
    /*
     I decided to put all fragrance and user related database operations in this single class for simplicity.
     Also implemented singleton pattern for user class to maintain current user state across the application.
     And my database design is very simple, just two tables, users and tuoksut (fragrances in Finnish).
     I joined them via userId foreign key in tuoksut table.
     */
    public class fragrance
    {
        public int newId { get; set; }
        //public int userid { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        //public string size { get; set; }
        public string notes { get; set; }
        public string category { get; set; }
        public string weather { get; set; }
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
       
        public static async Task GetUserId(string username, SqlConnection sql)
        { 
            var sqlcon = sql.ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
            string sqlQuery = "use fragrances; select id from users where username = @Username;";
            string sqlQuery2 = "use fragrances; select userId from tuoksut where userId = @Id";
            //string sqlQuery3 = "use fragrances; select * from tuoksut where userId = @Id";

            try
            {
                await sql.OpenAsync();
                var userList = (await sql.QueryAsync<users>(sqlQuery, new { Username = username })).ToList();
                foreach (var user in userList)
                {
                    
                    users.Instance.id = user.id;
  
                        var realId = (await sql.QueryAsync<users>(sqlQuery2, new { Id = users.Instance.id })).ToList();
                        realId.Equals(users.Instance.id);

                }
               
                sql.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(" An error occurred 1: " + ex.Message);
            }

        }
        public static async Task AddFragrancesAsync(SqlConnection sql)
        {
            var sqlcon = sql.ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

            string sqlQuery = "INSERT INTO tuoksut (userId, Name, Brand, notes, category, weather, occasion) VALUES (@UserId, @Name, @Brand, @notes, @category, @weather, @occasion);";
            
            sql.OpenAsync();
 
            
            sql.ExecuteAsync(sqlQuery, new
            {
                UserId = users.Instance.id,
                Name = "Test Fragrance",
                Brand = "Test Brand",
                notes = "Citrus, Woody",
                category = "Eau de Parfum",
                weather = "Warm",
                occasion = "Casual"
            });

        }
        public static async Task UpdateFragrance()
        {
            Console.WriteLine(" Updating a fragrance by ID...");
            await TestConnection();
        }
        public static async Task RemoveFragrance()
        {
            Console.WriteLine(" Removing a fragrance by ID...");

            await TestConnection();

        }
        public static async Task ListFragrancesForCurrentUser(SqlConnection sql,string username)
        {
            var sqlcon = sql.ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
            await GetUserId(username, sql);  
            
            await sql.OpenAsync();
            string sqlQuery3 = "use fragrances; select * from tuoksut where userId = @Id";

            var frags = await sql.QueryAsync<fragrance>(sqlQuery3, new { Id = users.Instance.id });
            
            foreach (var fragrance in frags)
            {
                Console.WriteLine($" Fragrance ID: {fragrance.newId}, Name: {fragrance.Name}, Brand: {fragrance.Brand}, Notes: {fragrance.notes}, Category: {fragrance.category}, Most Common weather: {fragrance.weather}, Occasion: {fragrance.occasion}");
            }

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
        public static bool emailExists = false;
        public static bool passwordExists = false;
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
            string sqlQuery = "SELECT COUNT(1) FROM users WHERE email = @Email;";
            try
            { 
                int rowsaffected = await sql.ExecuteScalarAsync<int>(sqlQuery, new { Email = email });
                if (rowsaffected <= 0 )
                {             
                    emailExists = false;
                }
                else
                {
                    emailExists = true;
                    Console.WriteLine(" Email is already in use. Please use a different email.");                 
                }
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
        // This method is not in use currently, might be useful later.
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
        public static async Task VerifyPasswordForCurrentUserAsync(string password, string username, SqlConnection sql)
        {
            var sqlcon = sql.ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
            string sqlQuery = "use fragrances; SELECT * FROM users WHERE username = @Username;";
            try
            {
                await sql.OpenAsync();
                var userList = (await sql.QueryAsync<users>(sqlQuery, new { Username = username })).ToList();
                foreach (var user in userList)
                {
                    if (user.username == username)
                    {
                        bool isPasswordValid = PasswordHasher.VerifyPassword(password, user.PasswordHash, Convert.FromHexString(user.salt));
                        if (isPasswordValid)
                        {
                            Console.WriteLine(" Password is valid. User authenticated.");
                            passwordExists = true;
                        }
                        else
                        {
                            Console.WriteLine(" Invalid password. Authentication failed.");
                            passwordExists = false;
                        }
                    }
                }
                sql.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(" An error occurred 1: " + ex.Message);
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
