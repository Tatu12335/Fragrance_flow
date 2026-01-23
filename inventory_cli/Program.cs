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
        if(string.IsNullOrEmpty(username))
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
                        default:
                            Console.WriteLine(" Invalid command. Type 'help' to see available commands.");
                            break;
                    }
                
                
            }
        }
        catch(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" An error occurred: " + ex.Message);
            Console.ResetColor();
        }
        Console.WriteLine("hello");

        
    }
    static void ShowPrompt()
    {
        Console.WriteLine(" Please enter a command to manage your fragrance inventory.");
        Console.WriteLine(" Available commands:");
        Console.WriteLine(" 1. add - Add a new fragrance");
        Console.WriteLine(" 2. list - List all fragrances");
        Console.WriteLine(" 3. remove - Remove a fragrance by ID");
        Console.WriteLine(" 4. help - Show this prompt");
    }

}
