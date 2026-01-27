using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuoksu_inventory.classes
{
    public class AdminPanel
    {
        public static AdminPanel Instance { get; } = new AdminPanel();
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public string AdminEmail { get; set; }
    }
}
