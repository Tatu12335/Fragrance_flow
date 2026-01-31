using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuoksu_inventory.classes
{
    public class AdminPanel
    {
        // NOTE TO ME : Continue working on the adminpanel.
        public static AdminPanel Instance { get; } = new AdminPanel();
        public int Id { get; set; }
        public string AdminName { get; set; }
        public string AdminEmail { get; set; }

        public static async Task AdminCommands()
        {
          
           
            string input = Console.ReadLine();
            string[] parts = input.Split(' ');
            string command = parts[0].ToLower();
            if (string.IsNullOrEmpty(command))
            {
                Console.WriteLine(" Command cant be empty");
                Environment.Exit(0);
            }
           switch (command)
           {
                case "view-users":
                    break;
                case "ban":
                   
                    
                    break;
                default:
                    Console.WriteLine(" Invalid command");
                    Environment.Exit(0);
                    break;
            }


        }
        public static void LoadAdminPanel()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" Welcome to the admin-panel! ");
            Console.WriteLine("");
            Console.WriteLine("Options : ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" \"view-users\" - shows all registered users in the database");
            Console.WriteLine(" \"remove --userId\" - removes given user from the users database");
            Console.WriteLine(" \"ban --userId\" - Bans a given user");
            Console.WriteLine("\"unban --userId\" Unbans a given user");
            Console.WriteLine("\"promote --userId\" Promote user to admin");
            Console.WriteLine("\"demote --userId\" Demote user to have normal privilages");
            Console.ResetColor();
        }
    }
}
