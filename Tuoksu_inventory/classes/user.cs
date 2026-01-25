using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuoksu_inventory.classes
{
    public class users
    {
        public static users Instance { get; } = new users();
        public int id { get; set; }
        public string username { get; set; }
        public string PasswordHash { get; set; }
        public string salt { get; set; }

    }
}
