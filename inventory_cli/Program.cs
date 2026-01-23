//hours wasted writing, debugging and learning sql : 4hrs 30mins

using Tuoksu_inventory.classes;

class Program
{
    static void Main(string[] args)
    {
        fragrance fragrance = new fragrance();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("-------------------------");
        Console.WriteLine(" Fragrance flow");
        Console.WriteLine("-------------------------");
        Console.WriteLine(" Sign in With a username");
        Console.Write(">");
        var username = Console.ReadLine();
        if (string.IsNullOrEmpty(username))
        {
            Console.WriteLine(" Username cannot be empty. Exiting application.");

            return;
        }
        else
        {

        }

        ShowPrompt();
        Console.ResetColor();
        string input = Console.ReadLine().ToLower();
        try
        {
            Console.Write(">");
            if (input == null)
            {
                input = Console.ReadLine().ToLower();
            }
            else
            {
                input.ToLower();
            }
            switch (input)
            {
                case "add":
                    fragrance.AddFragrance();
                    break;
                case "list":
                    Console.WriteLine(" Listing all fragrances...");
                    fragrance.ListFragrances();
                    break;
                case "remove":
                    Console.WriteLine(" Removing a fragrance by ID...");
                    // Implementation for removing a fragrance goes here
                    break;
                case "help":
                    ShowPrompt();
                    break;
                case "createuser":
                    Console.WriteLine(" enter username to create");
                    var newusername = Console.ReadLine();
                    newusername = Convert.ToString(newusername);
                    Console.WriteLine($"is this the username you want => {newusername}");
                    switch (Console.ReadLine().ToLower())
                    {
                        case "yes":
                            break;
                        case "no":
                            Console.WriteLine(" enter username to create");
                            newusername = Console.ReadLine();
                            newusername = Convert.ToString(newusername);
                            break;
                    }   
                        
                   Console.WriteLine(" enter password to create");

                    var newpassword = Console.ReadLine();
                    newpassword = Convert.ToString(newpassword);

                    fragrance.CreateUser(newusername, newpassword);
                    break;
                default:
                    Console.WriteLine(" Invalid command. Type 'help' to see available commands.");
                    break;
            }
            
            
                           
        }




        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" An error occurred: " + ex.Message);
            Console.ResetColor();
        }
    }      

        
    
    static void ShowPrompt()
    {
        Console.WriteLine(" Please enter a command to manage your fragrance inventory.");
        Console.WriteLine(" Available commands:");
        Console.WriteLine(" add - Add a new fragrance");
        Console.WriteLine(" list - List all fragrances");
        Console.WriteLine(" remove - Remove a fragrance by ID");
        Console.WriteLine(" help - Show this prompt");
        Console.WriteLine(" createuser");
    }

}
