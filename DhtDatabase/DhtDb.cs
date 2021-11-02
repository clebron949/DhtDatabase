using System;
using System.Collections.Generic;
using Npgsql;

namespace DhtDatabase
{
    public class DhtDb : DhtModel
    {
       public string ErrorMessage { get; set; }

        const string server = "ec2-34-232-191-133.compute-1.amazonaws.com";
        const string username = "ubygjyqvmxllej";
        const string password = "b87d3e5c9555275f73d967a8727cfe72ecb3ae886fc7272149ae9ee4bccb9bc1";
        const string database = "d7rmihdgqufrsl";
        const string port = "5432";
        private readonly NpgsqlConnection _connection;

        public DhtDb()
        {
            var connectionStr = $"Host={server};Username={username};Password={password};Database={database};Port={port};Pooling=true;SSL Mode=Require;TrustServerCertificate=True;";
            _connection = new NpgsqlConnection(connectionStr);

            Id = -1;
            Date = DateTime.Now;
            TemperatureInC = 25;
            TemperatureInF = 77;
            Humidity = 100;
        }

        public bool CloseDb()
        {
            try
            {
                _connection.Close();
                return true;
            }
            catch (Exception error)
            {
                ErrorMessage = error.Message;
                return false;
            }
        }

        public List<DhtModel> ReadAllData()
        {
            List<DhtModel> dht22List = new List<DhtModel>();       
            var sql = "SELECT * FROM dht22";

            _connection.Open();
            var cmd = new NpgsqlCommand(sql, _connection);

            NpgsqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                DhtModel dht22Data = new DhtModel()
                {
                    Id = rdr.GetInt32(0),
                    Date = rdr.GetTimeStamp(1).ToDateTime(),
                    TemperatureInC = rdr.GetFloat(2),
                    TemperatureInF = rdr.GetFloat(3),
                    Humidity = rdr.GetFloat(4)
                };
                dht22List.Add(dht22Data);
            }

            _connection.Close();
            return dht22List;
        }

        public bool Update()
        {
            _connection.Open();

            var sql = "UPDATE dht22 SET temperatureinc = :temperatureinc, temperatureinf = :temperatureinf, humidity = :humidity  WHERE id = :id";
            var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("temperatureinc", TemperatureInC);
            cmd.Parameters.AddWithValue("temperatureinf", TemperatureInF);
            cmd.Parameters.AddWithValue("humidity", Humidity);
            cmd.Parameters.AddWithValue("id", Id);
            cmd.Prepare();

            var rowsAffected = cmd.ExecuteNonQuery();

            _connection.Close();

            return rowsAffected > 0;
        }

        public bool Insert()
        {
            Date = DateTime.Now;

            _connection.Open();
            var sql = $"INSERT INTO dht22(date,temperatureinc, temperatureinf, humidity)"
            + " VALUES(@date, @temperatureinc, @temperatureinf, @humidity)";
            var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("date", Date);
            cmd.Parameters.AddWithValue("temperatureinc", TemperatureInC);
            cmd.Parameters.AddWithValue("temperatureinf", TemperatureInF);
            cmd.Parameters.AddWithValue("humidity", Humidity);
            cmd.Prepare();

            var rowsAffected = cmd.ExecuteNonQuery();

            _connection.Close();
            return rowsAffected > 0;
        }

        public bool Delete()
        {
            _connection.Open();

            var sql = "DELETE FROM dht22 WHERE id = :id";
            var cmd = new NpgsqlCommand(sql, _connection);
            cmd.Parameters.AddWithValue("id", Id);
            cmd.Prepare();

            var rowsAffected = cmd.ExecuteNonQuery();

            _connection.Close();
            return rowsAffected > 0;
        }
    }
}