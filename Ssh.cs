using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Renci.SshNet;
using MySql.Data.MySqlClient;

namespace RandomNumberApp
{
    class Ssh
    {

        private readonly SshClient _client;
        private readonly Dictionary<string, string> _config;

        public Ssh()
        {
            _config = GetSettings("ssh.xml");
            _client = new SshClient("boss.staszic.waw.pl", "torisels", new PrivateKeyFile("id_rsa", _config["sshpass"]));
            _client.Connect();
            ForwardPort();
        }

        private void ForwardPort()
        {
            if (!_client.IsConnected) return;
            var portForwarded = new ForwardedPortLocal("127.0.0.1", 3307, "mysql.staszic.waw.pl", 3306);
            _client.AddForwardedPort(portForwarded);
            portForwarded.Start();
        }

        public Sql EstablishMySqlConnection()
        {
            MySqlConnectionStringBuilder connString = new MySqlConnectionStringBuilder();
            connString.Server = _config["dbhost"];
            connString.UserID = _config["dbuser"];
            connString.Password = _config["dbpass"];
            connString.Database = _config["dbname"];
            connString.Port = 3307;
            return new Sql(connString.ToString());
        }

        public Dictionary<string, string> GetSettings(string path)
        {
            var document = XDocument.Load(path);
            var root = document.Root;
            var results =
                root?.Elements()
                    .ToDictionary(element => element.Name.ToString(), element => element.Value);
            return results;
        }
    }
}
