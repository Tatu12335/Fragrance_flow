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
        public static AdminPanel Instance { get; } = new AdminPanel();
        public int Id { get; set; }
        public string AdminName { get; set; }
        public string AdminEmail { get; set; }

        public static async Task GetAdminPanelByIdAsync(SqlConnection sql)
        {
            
            
            
            
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
            Console.ResetColor();
        }
    }
}
