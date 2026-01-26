//hours wasted writing, debugging and learning sql : 10hrs 0mins

using System.Net.Http.Headers;
using Tuoksu_inventory.classes;

class Program
{
    static async Task Main(string[] args)
    {
        fragrance fragrance = new fragrance();
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
                // Sorry for the nested code, couldn't figure out a better way to do it quickly, might refactor later.
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
                            Console.WriteLine(" Please enter email for the new user");
                            Console.Write(">");
                            var email = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(email) && email.Contains('@'))
                            {
                                await fragrance.VerifyEmail(email, connection);
                            }
                            else
                            {
                                
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(" Email cannot be empty.");
                                Console.ResetColor();
                                return;
                            }

                            if (fragrance.emailExists)
                            {

                                Environment.Exit(0);
                                
                            }
                            else 
                            {
                                await fragrance.VerifyEmail(email, connection);
                                Console.WriteLine(" Please enter a password for the new user:");
                                Console.Write(">");
                                var password = Console.ReadLine();

                                if (string.IsNullOrWhiteSpace(password))
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine(" Password cannot be empty.");
                                    Console.ResetColor();
                                    return;
                                }
                                else
                                {

                                    await fragrance.CreateUser(username, password, email);

                                    Console.ResetColor();
                                    return;
                                }
                                
                            }
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
                    var passwordAttempt = Console.ReadLine();
                    await fragrance.VerifyPasswordForCurrentUserAsync(passwordAttempt, username, connection);
                    Console.ForegroundColor = ConsoleColor.Green;
                    if(fragrance.passwordExists)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"------ WELCOME BACK [{username}] ------");
                        ShowPrompt();
                        var command = Console.ReadLine().ToLower();
                        switch (command)
                        {
                            case "add":
                                await fragrance.AddFragrancesAsync(connection);
                                break;
                            case "list":
                                await fragrance.ListFragrancesForCurrentUser(connection,username);
                                break;
                            case "remove":
                                await fragrance.RemoveFragrance();
                                break;
                            case "help":
                                ShowPrompt();
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



        static void ShowPrompt()
        {
            Console.WriteLine("");
            Console.WriteLine(" Please enter a command to manage your fragrance inventory.");
            Console.WriteLine(" Available commands:");
            Console.WriteLine(" add - Add a new fragrance");
            Console.WriteLine(" list - List all fragrances");
            Console.WriteLine(" remove - Remove a fragrance by ID");
            Console.WriteLine(" help - Show this prompt");
            Console.WriteLine("");
            
        }

    }
}
