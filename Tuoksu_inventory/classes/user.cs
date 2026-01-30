namespace Tuoksu_inventory.classes
{
    public class users
    {
        public static users Instance { get; } = new users();
        public int id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string PasswordHash { get; set; }
        public string salt { get; set; }
        public int isBanned { get; set; } = 0;
        public int isAdmin { get; set; } = 0;

    }
}
