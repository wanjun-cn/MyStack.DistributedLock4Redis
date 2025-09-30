using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DistributedLock4Redis.Internal
{
    internal class RedisConnectionString
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 6379;
        public string? Password { get; set; }
        public int Database { get; set; } = 0;
        public int ConnectTimeout { get; set; } = 5000;
        public int SyncTimeout { get; set; } = 5000;
        public bool AllowAdmin { get; set; } = false;
        public bool Ssl { get; set; } = false;

        public static RedisConnectionString Parse(string connectionString)
        {
            var config = new RedisConnectionString();

            if (string.IsNullOrWhiteSpace(connectionString))
                return config;

            var parts = connectionString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                var keyValue = part.Split(new[] { '=' }, 2);
                if (keyValue.Length != 2) continue;

                var key = keyValue[0].Trim().ToLower();
                var value = keyValue[1].Trim();

                switch (key)
                {
                    case "host":
                    case "server":
                        config.Host = value;
                        break;
                    case "port":
                        if (int.TryParse(value, out int port))
                            config.Port = port;
                        break;
                    case "password":
                    case "pwd":
                        config.Password = value;
                        break;
                    case "database":
                    case "db":
                        if (int.TryParse(value, out int db))
                            config.Database = db;
                        break;
                    case "connecttimeout":
                    case "connectTimeout":
                        if (int.TryParse(value, out int connectTimeout))
                            config.ConnectTimeout = connectTimeout;
                        break;
                    case "synctimeout":
                    case "syncTimeout":
                        if (int.TryParse(value, out int syncTimeout))
                            config.SyncTimeout = syncTimeout;
                        break;
                    case "allowadmin":
                        if (bool.TryParse(value, out bool allowAdmin))
                            config.AllowAdmin = allowAdmin;
                        break;
                    case "ssl":
                        if (bool.TryParse(value, out bool ssl))
                            config.Ssl = ssl;
                        break;
                }
            }

            return config;
        }

        public override string ToString()
        {
            var parts = new List<string>
            {
                $"host={Host}",
                $"port={Port}",
                $"database={Database}",
                $"connectTimeout={ConnectTimeout}",
                $"syncTimeout={SyncTimeout}",
                $"allowAdmin={AllowAdmin}",
                $"ssl={Ssl}"
            };

            if (!string.IsNullOrEmpty(Password))
            {
                parts.Add($"password={Password}");
            }

            return string.Join(",", parts);
        }
    }
}
