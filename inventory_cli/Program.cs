//hours wasted writing, debugging and learning sql,wpf etc : 19hrs 0mins

using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Diagnostics.Metrics;
using System.Text;
using Tuoksu_inventory.classes;

class Program
{
    public static fragrance fragrance = new fragrance();
    static async Task Main(string[] args)
    {
        //fragrance fragrance = new fragrance();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("-------------------------");
        Console.WriteLine(" Fragrance flow");
        Console.WriteLine("-------------------------");
        Console.WriteLine(" Sign in With a username");
        Console.Write(">");
        var username = Console.ReadLine();

        if (username == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" Username cannot be empty.");
            Console.ResetColor();
            return;
        }
        else
        {
            using (var connection = fragrance.CONNECTIONHELPER())
            {
                
                await fragrance.CheckIfUserExists(username, connection);
                if (!fragrance.userExists)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(" User does not exist. Do you want to create one(Y/N)?");
                    Console.ResetColor();
                    Console.Write(">");
                    switch (Console.ReadLine().ToLower())
                    {
                        case "n":
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(" Exiting application.");
                            Console.ResetColor();
                            return;
                        case "y":                    
                              await CreateUserAsync();
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(" Invalid option. Exiting application.");
                            Console.ResetColor();
                            return;

                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(" User found. Please enter your password to continue.");
                    

                    await fragrance.VerifyPasswordForCurrentUserAsync(ReadPassword(), username, connection);
                    await fragrance.IsAdmin(connection,username);

                    if(users.Instance.isAdmin == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("------ WELCOME ADMIN ------");
                        Console.ResetColor();
                        
                        AdminPanel.LoadAdminPanel();
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    if (fragrance.passwordExists)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"------ WELCOME BACK [{username}] ------");
                        ShowPrompt();
                        var command = Console.ReadLine().ToLower();
                        switch (command)
                        {
                            case "add":
                                try
                                {
                                    await fragrance.AddFragrancesAsync(connection);
                                }
                                catch (Exception ex)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($" Error adding fragrance: {ex.Message}");
                                    Console.ResetColor();
                                }
                                break;
                            
                            case "list":
                                try
                                {
                                    await fragrance.ListFragrancesForCurrentUser(connection, username);
                                }
                                catch (Exception ex)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($" Error listing fragrances: {ex.Message}");
                                    Console.ResetColor();
                                }
                                break;
                        
                            case "remove":
                                try
                                {
                                    await fragrance.RemoveFragranceAsync(connection, users.Instance.id,username);
                                }
                                catch (Exception ex)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($" Error removing fragrance: {ex.Message}");
                                    Console.ResetColor();
                                }
                                break;
                            
                            case "help":
                                ShowPrompt();
                                break;
                            case "suggest":
                                try
                                {
                                    await fragrance.FragranceForWeather(connection, users.Instance.id);
                                }
                                catch (Exception ex)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine($" Error suggesting fragrance: {ex.Message}");
                                    Console.ResetColor();
                                }
                                break;
                            default:
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(" Invalid command. Type 'help' to see available commands.");
                                break;
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.ResetColor();
                        return;
                    }

                    Console.ResetColor();
                }
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
        }
        static async Task CreateUserAsync()
        {
            
            using (var connection = fragrance.CONNECTIONHELPER())
            {
                Console.WriteLine(" Please enter a username for the new user:");
                Console.Write(">");
                var username = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(username))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" Username cannot be empty.");
                    Console.ResetColor();
                    return;
                }
                 fragrance.CheckIfUserExists(username, connection).Wait();


                if (fragrance.userExists)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" Username already exists. Please choose a different username.");
                    Console.ResetColor();
                    return ;
                }

                Console.WriteLine(" Please enter email for the new user");
                Console.Write(">");
                var email = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                {

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" Email is invalid.");
                    Console.ResetColor();
                    return;
                }
                
               fragrance.VerifyEmail(email, connection).Wait();
                
                if (fragrance.emailExists)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" Email already exists. Please choose a different email.");
                    Console.ResetColor();
                    return ;
                }

                Console.WriteLine(" Please enter a password for the new user:");
                Console.Write(">");
                var password = ReadPassword();

                if (string.IsNullOrWhiteSpace(password))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(" Password cannot be empty.");
                    Console.ResetColor();
                    return;
                }
               
                    await fragrance.CreateUser(username, password, email);
                    Console.ResetColor();
                    
                
            }
        }
        static string ReadPassword()
        {
            StringBuilder password = new StringBuilder();
            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key != ConsoleKey.Backspace && keyInfo.Key != ConsoleKey.Enter)
                {
                    password.Append(keyInfo.KeyChar);
                    Console.Write("*");
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                    
                }
            } while (keyInfo.Key != ConsoleKey.Enter);
            Console.WriteLine();
            return password.ToString();
        }
        static void ShowPrompt()
        {
            Console.WriteLine("");
            Console.WriteLine(" Please enter a command to manage your fragrance inventory.");
            Console.WriteLine(" Available commands:");
            Console.WriteLine(" add - Add a new fragrance");
            Console.WriteLine(" list - List all fragrances");
            Console.WriteLine(" remove - Remove a fragrance by ID");
            Console.WriteLine(" suggest - Get fragrance suggestions based on the weather\n( NOTE : The program gets your location automatically if you use an vpn it wont work correctly)\n");
            Console.WriteLine(" help - Show this prompt");
            Console.WriteLine("");

        }

    }
}
