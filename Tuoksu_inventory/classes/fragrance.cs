using Dapper;
using Microsoft.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http.Json;
using System.Globalization;
using Microsoft.Identity.Client;
namespace Tuoksu_inventory.classes
{
    /*
     I decided to put all fragrance and user related database operations in this single class for simplicity.
     Also implemented singleton pattern for user class to maintain current user state across the application.
     And my database design is very simple, just two tables, users and tuoksut (fragrances in Finnish).
     I joined them via userId foreign key in tuoksut table.
     Not sure if this is the best practice but it works for this simple CLI application.
     
     */
    public class fragrance
    {
        public static fragrance Instance { get; } = new fragrance();
        // did not want to create multiple instances of HttpClient
        private static readonly HttpClient client = new HttpClient();
        public int id { get; set; }
        //public int userid { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        //public string size { get; set; }
        public string notes { get; set; }
        public string category { get; set; }
        public string weather { get; set; }
        public string occasion { get; set; }

        // note to self: implement async/await properly later.

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
        // Method to get user ID based on username
        public static async Task GetUserId(string username, SqlConnection sql)
        {
            var sqlcon = sql.ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
            string sqlQuery = "select id from users where username = @Username;";
            string sqlQuery2 = "select userId from tuoksut where userId = @Id";
            

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
        // Method to add a new fragrance to the database
        public static async Task AddFragrancesAsync(SqlConnection sql)
        {
            var sqlcon = sql.ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

            string sqlQuery = "INSERT INTO tuoksut (userId, Name, Brand, notes, category, weather, occasion) VALUES (@UserId, @Name, @Brand, @notes, @category, @weather, @occasion);";
            await GetUserId(users.Instance.username, sql);
            await sql.OpenAsync();

            Console.WriteLine(" Whats the name of the fragrance");
            Console.Write(">");
            var name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine(" Name cannot be empty. Aborting fragrance addition.");
                return;
            }

            Console.WriteLine(" Whats the brand of the fragrance");
            Console.Write(">");
            var brand = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(brand))
            {
                Console.WriteLine(" Brand cannot be empty. Aborting fragrance addition.");
                return;
            }

            Console.WriteLine(" List some notes (comma separated)");
            Console.Write(">");
            var notes = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(notes))
            {
                Console.WriteLine(" Notes cannot be empty. Aborting fragrance addition.");
                return;
            }

            Console.WriteLine(" Whats the category (gourmand,fresh,amber,etc.)");
            Console.Write(">");
            var category = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(category))
            {
                Console.WriteLine(" Category cannot be empty. Aborting fragrance addition.");
                return;
            }

            Console.WriteLine(" Most common weather to wear it in (cold, warm, etc.)");
            Console.Write(">");
            var weather = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(weather))
            {
                Console.WriteLine(" Weather cannot be empty. Aborting fragrance addition.");
                return;
            }

            Console.WriteLine(" Most common occasion to wear it in (casual, formal, etc.)");
            Console.Write(">");
            var occasion = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(occasion))
            {
                Console.WriteLine(" Occasion cannot be empty. Aborting fragrance addition.");
                return;
            }





            await sql.ExecuteAsync(sqlQuery, new
            {
                UserId = users.Instance.id,
                Name = name,
                Brand = brand,
                notes = notes,
                category = category,
                weather = weather,
                occasion = occasion
            });
            Console.WriteLine(" Fragrance added successfully.");
            sql.Close();
        }
        public static async Task UpdateFragrance()
        {
            Console.WriteLine(" Updating a fragrance by ID...");
            await TestConnection();
        }
        // remove fragrance by id, only if it belongs to the current user
        public static async Task RemoveFragranceAsync(SqlConnection sql, int id, string username)
        {
            await GetUserId(username, sql);

            var sqlcon = sql.ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

            Console.WriteLine(" Enter the id of the fragrance you want removed");
            Console.Write(">");
            id = Convert.ToInt16(Console.ReadLine());
            var sqlQuery = "DELETE FROM tuoksut WHERE id = @Id AND userId = @UserId;";


            try
            {
                await sql.OpenAsync();

                try
                {
                    var fragranceToDelete = await sql.QueryFirstOrDefaultAsync<fragrance>("SELECT * FROM tuoksut WHERE id = @Id AND userId = @UserId;", new { Id = id, UserId = users.Instance.id });
                    if (fragranceToDelete == null)
                    {
                        Console.WriteLine(" Fragrance not found or does not belong to the current user. Aborting removal.");
                        return;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine(" Fragrance ownership verified.");
                        await sql.ExecuteAsync(sqlQuery, new { Id = id, UserId = users.Instance.id });
                        Console.WriteLine(" Fragrance removed successfully.");
                        Console.ResetColor();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" An error occurred while verifying fragrance ownership: " + ex.Message);
                    return;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(" An error occurred while opening connection: " + ex.Message);
            }
            finally
            {
                await sql.CloseAsync();
            }
        }
        // List all fragrances for the current user
        public static async Task ListFragrancesForCurrentUser(SqlConnection sql, string username)
        {

            var sqlcon = sql.ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
            await GetUserId(username, sql);
            try
            {
                await sql.OpenAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(" An error occurred while opening the connection: " + ex.Message);
            }
            string sqlQuery3 = "use fragrances; select * from tuoksut where userId = @Id";
            try
            {
                var frags = await sql.QueryAsync<fragrance>(sqlQuery3, new { Id = users.Instance.id });
                foreach (var fragrance in frags)
                {
                    Console.WriteLine($" Fragrance ID: {fragrance.id}, Name: {fragrance.Name}, Brand: {fragrance.Brand}, Notes: {fragrance.notes}, Category: {fragrance.category}, Most Common weather: {fragrance.weather}, Occasion: {fragrance.occasion}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(" An error occurred while listing fragrances: " + ex.Message);
            }


        }
        // Debugging method for connection strings, i needed to debug my connection string issues multiple times. 
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
        // Global flags for user existence checks
        public static bool userExists = false;
        public static bool emailExists = false;
        public static bool passwordExists = false;
        //
        // Method to check if a user exists in the database
        public static async Task CheckIfUserExists(string username, SqlConnection sqlConnection)
        {

            var sqlcon = sqlConnection.ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
            string sqlQuery = "SELECT * FROM users WHERE username = @Username;";

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
        // Method to create and return a SQL connection, I figured it would be cleaner this way.
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
        // Method to create a new user in the database
        public static async Task CreateUser(string username, string password, string email)
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
                        Email = email,
                        Username = username,
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
        // Method to verify if an email is already in use
        public static async Task VerifyEmail(string email, SqlConnection sql)
        {
            var sqlcon = sql.ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
            string sqlQuery = "SELECT COUNT(1) FROM users WHERE email = @Email;";
            try
            {
                int rowsaffected = await sql.ExecuteScalarAsync<int>(sqlQuery, new { Email = email });
                if (rowsaffected <= 0)
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
        public static async Task<int> SetId(string username, SqlConnection sql)
        {
            string sqlQuery = "select top 1 * from users order by id desc;";
            try
            {
                await sql.OpenAsync();
                var userList = (await sql.QueryAsync<users>(sqlQuery)).ToList();
                foreach (var user in userList)
                {
                    if (user.username == username)
                    {
                        users.Instance.id = user.id++;
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
        // Method to verify the password for the current user
        public static async Task VerifyPasswordForCurrentUserAsync(string password, string username, SqlConnection sql)
        {
            var sqlcon = sql.ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
            string sqlQuery = "SELECT * FROM users WHERE username = @Username;";
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
        
        // Method to fetch and display the user's IP location
        public static async Task UserLocation()
        {
            

            try
            {
                var location = await client.GetFromJsonAsync<IpLocation>("http://ip-api.com/json/");
                
                if (location != null && location.status == "success")
                {                        

                    var weatherUrl = $" https://api.open-meteo.com/v1/forecast?latitude={location.lat.ToString(CultureInfo.InvariantCulture)}&longitude={location.lon.ToString(CultureInfo.InvariantCulture)}&current_weather=true&temperature_unit=celsius";
                        
                        var response = await client.GetAsync(weatherUrl);
                    if (!response.IsSuccessStatusCode) return;

                    var json = await response.Content.ReadAsStringAsync();

                    using var document = System.Text.Json.JsonDocument.Parse(json);
                    var currentWeather = document.RootElement
                        .GetProperty("current_weather");
                        
                    var temperature = currentWeather
                        .GetProperty("temperature").GetDouble();    

                }
                else
                {
                    Console.WriteLine(" Unable to fetch IP location.");
                    Console.WriteLine($" Status: {location.status}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(" An error occurred while fetching IP location: " + ex.Message);
            }
        }
        // Method to suggest fragrances based on the weather
        public static async Task FragranceForWeather(SqlConnection sql,double temperature)
        {
            var sqlcon = sql.ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
            await GetUserId(users.Instance.username, sql);

            var sqlQuery = "select * from tuoksut where userId = @Id and (category = 'gourmand' OR category = 'amber');";

            try
            {
                await UserLocation();
                if (temperature <= 10)
                {
                    Console.WriteLine(" It's cold outside. You might want to wear warm and spicy fragrances. For example:\n");
                    await sql.OpenAsync();
                    var frags = await sql.QueryAsync<fragrance>(sqlQuery, new { Id = users.Instance.id });
                    foreach (var fragrance in frags)
                    {
                        Console.WriteLine($" Name: {fragrance.Name}, Brand: {fragrance.Brand}, Notes: {fragrance.notes},");
                    }

                }
                else if (temperature > 10 && temperature <= 20)
                {
                    Console.WriteLine(" It's mild outside. You might want to wear fresh and floral fragrances. For example");
                    sqlQuery = "select * from tuoksut where userId = @Id and (category = 'floral' or category = 'fresh');";

                    await sql.OpenAsync();
                    var frags = await sql.QueryAsync<fragrance>(sqlQuery, new { Id = users.Instance.id });
                    foreach (var fragrance in frags)
                    {
                        Console.WriteLine($" Name: {fragrance.Name}, Brand: {fragrance.Brand}, Notes: {fragrance.notes},");
                    }
                }
                else
                {
                    sqlQuery = "select * from tuoksut where userId = @Id and (category = 'citrusy' or category = 'aquatic' or 'fresh');";
                    Console.WriteLine(" It's warm outside. You might want to wear light and citrusy fragrances. For Example\n");

                    await sql.OpenAsync();
                    var frags = await sql.QueryAsync<fragrance>(sqlQuery, new { Id = users.Instance.id });
                    foreach (var fragrance in frags)
                    {
                        Console.WriteLine($" Name: {fragrance.Name}, Brand: {fragrance.Brand}, Notes: {fragrance.notes},");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(" An error occurred while fetching fragrance for weather: " + ex.Message);
            }
        }
        // Additional classes can be added here as needed
        public class IpLocation
        {
           public static IpLocation Instance { get; } = new IpLocation();
            public  string status { get; set; }
            public string country { get; set; }
            public  string countryCode { get; set; }
            public  string region { get; set; }
            public  string regionName { get; set; }
            public  string city { get; set; }
            public  string zip { get; set; }
            public  float lat { get; set; }
            public  float lon { get; set; }
            public  string timezone { get; set; }
            public  string isp { get; set; }
            public  string org { get; set; }
            public  string @as { get; set; }
            public  string query { get; set; }
        }

        public class Algorithms
        {

        }
        // Static class for password hashing and verification
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
                // Used fixed time comparison to prevent timing attacks
                return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
            }
        }

    }
}