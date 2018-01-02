using MySql.Data.MySqlClient;

namespace RandomNumberApp
{
    class Sql
    {
        private readonly MySqlConnection _connection;

        public Sql(string connectionString)
        {
            _connection = new MySqlConnection(connectionString);     
            _connection.Open();
        }

        public int GetBellOffset()
        {
            using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM bell_offest", _connection))
            {
                using (var reader = cmd.ExecuteReader())
                { 
                    while (reader.Read())
                        return reader.GetInt32(0);
                }
            }
            return 0;
        }
    }
}
