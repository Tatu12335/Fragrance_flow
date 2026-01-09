//hours wasted 0.30mins

using Tuoksu_inventory.classes;

class Program
{
    static void Main(string[] args)
    {
        fragrance fragrance = new fragrance();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("-------------------------");
        Console.WriteLine("Write down your fragrances");
        Console.WriteLine("-------------------------");
        Console.ResetColor();
        string input = Console.ReadLine();
        try
        {
            

            if (args.Length > 0)
            {
                input = args[0].ToLower();
                ShowPrompt();
                return;
            }
            else
            {
                
                ShowPrompt();
                Console.Write(">");
                
                
            }
        }
        catch(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" An error occurred: " + ex.Message);
            Console.ResetColor();
        }

        
    }
    public void AddFragrance()
    {
        Console.WriteLine(" Adding a new fragrance...");
        // Implementation for adding a fragrance goes here
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
