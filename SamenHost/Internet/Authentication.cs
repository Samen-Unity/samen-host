using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SamenHost.Internet
{

    /// <summary>
    /// Manager for accounts
    /// </summary>
    public class Authentication
    {

        /// <summary>
        /// Load all accounts from a file
        /// </summary>
        public static void LoadFile()
        {
            if(!File.Exists("accounts.json"))
            {
                Logging.Log("Authentication", "Creating default login file. Make sure to edit before using!", LogType.IMPORTANT);
                string defaultContent = @"
[
    {
        ""username"": ""Visitor"",
        ""password"": ""Welcome"",
        ""permissions"": [""example.permission""]
    }
]";
                File.WriteAllText("accounts.json", defaultContent);
            }

            accounts = JsonSerializer.Deserialize<List<UserAccount>>(File.ReadAllText("accounts.json"));
            Logging.Log("Authentication", $"Loaded {accounts.Count} accounts.", LogType.INFO);
        }


        static List<UserAccount> accounts;

        /// <summary>
        /// Get a specific account from a username and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>The account if found, otherwise null</returns>
        public static UserAccount GetAccount(string username, string password)
        {
            return accounts.FirstOrDefault((account) =>
            {
                return account.username == username && account.password == password;
            });
        }
    }

    /// <summary>
    /// An account of a user
    /// </summary>
    public class UserAccount
    {
        /// <summary>
        /// The username of the account
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// The password of the account
        /// </summary>
        public string password { get; set; }

        /// <summary>
        /// The permissions for the account
        /// </summary>
        public string[] permissions { get; set; }
    }
}
