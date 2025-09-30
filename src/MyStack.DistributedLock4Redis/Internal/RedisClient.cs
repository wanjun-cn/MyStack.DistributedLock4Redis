using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DistributedLock4Redis.Internal
{
    internal class RedisClient
    {
        private static RedisConnectionString? _connectionString;
        private static bool _initialized = false;
        public static void Initialize(string connectionString)
        {
            if (_initialized) return;

            _connectionString = RedisConnectionString.Parse(connectionString);
            Thread.MemoryBarrier();
            _initialized = true;
        }

        private static async Task<T> ConnectAndSendAsync<T>(Func<NetworkStream, Task<T>> func)
        {
            if (!_initialized || _connectionString == null)
                throw new InvalidOperationException("RedisClient has not been initialized. Call InitializeAsync first.");

            using var tcpClient = new TcpClient();
            tcpClient.SendTimeout = _connectionString.SyncTimeout;
            tcpClient.ReceiveTimeout = _connectionString.SyncTimeout;

            await tcpClient.ConnectAsync(_connectionString.Host, _connectionString.Port);
            using var stream = tcpClient.GetStream();

            if (!string.IsNullOrEmpty(_connectionString.Password))
            {
                await AuthenticateAsync(stream);
            }

            if (_connectionString.Database > 0)
            {
                await SelectDatabaseAsync(stream);
            }

            return await func(stream);
        }

        private static async Task AuthenticateAsync(NetworkStream stream)
        {
            await SendCommandAsync(stream, "AUTH", _connectionString!.Password!);
            var response = await ReadResponseAsync(stream);
            if (!IsSuccessResponse(response))
                throw new DistributedLock4RedisException("Redis authentication failed");
        }

        private static async Task SelectDatabaseAsync(NetworkStream stream)
        {
            await SendCommandAsync(stream, "SELECT", _connectionString!.Database.ToString());
            var response = await ReadResponseAsync(stream);
            if (!IsSuccessResponse(response))
                throw new DistributedLock4RedisException($"Failed to select database {_connectionString.Database}");
        }

        private static bool IsSuccessResponse(string? response)
        {
            return response == "OK" || (response?.StartsWith(":1") == true);
        }

        private static async Task SendCommandAsync(NetworkStream stream, params string[] args)
        {
            var commandBuilder = new StringBuilder();
            commandBuilder.Append($"*{args.Length}\r\n");

            foreach (var arg in args)
            {
                var argBytes = Encoding.UTF8.GetBytes(arg);
                commandBuilder.Append($"${argBytes.Length}\r\n");
                commandBuilder.Append($"{arg}\r\n");
            }

            var data = Encoding.UTF8.GetBytes(commandBuilder.ToString());
            await stream.WriteAsync(data, 0, data.Length);
        }

        private static async Task<string?> ReadResponseAsync(NetworkStream stream)
        {
            if (!_initialized)
                throw new InvalidOperationException("RedisClient has not been initialized. Call InitializeAsync first.");

            using var reader = new StreamReader(stream, Encoding.UTF8, false, 4096, true);
            var firstLine = await reader.ReadLineAsync();
            if (firstLine == null)
                throw new DistributedLock4RedisException("No response from Redis");

            return firstLine[0] switch
            {
                '+' => firstLine[1..], // Simple string
                '-' => throw new DistributedLock4RedisException($"Redis error: {firstLine[1..]}"),
                ':' => firstLine, // Integer
                '$' => await ParseBulkStringAsync(reader, firstLine),
                '*' => firstLine, // Array
                _ => throw new DistributedLock4RedisException($"Unsupported Redis response: {firstLine}")
            };
        }

        private static async Task<string?> ParseBulkStringAsync(StreamReader reader, string firstLine)
        {
            if (int.TryParse(firstLine.AsSpan(1), out int length))
            {
                if (length == -1)
                    return null; // nil response

                var data = await reader.ReadLineAsync();
                await reader.ReadLineAsync(); // Consume the trailing CRLF
                return data;
            }

            throw new DistributedLock4RedisException($"Invalid bulk string length: {firstLine}");
        }

        public static async Task<bool> SetAsync(string key, string value, int expirySeconds = 0, bool nx = false)
        {
            if (!_initialized)
                throw new InvalidOperationException("RedisClient has not been initialized. Call InitializeAsync first.");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
            if (value == null)
                throw new ArgumentException("Value cannot be null", nameof(value));

            var commandArgs = new List<string> { "SET", key, value };
            if (nx) commandArgs.Add("NX");
            if (expirySeconds > 0)
            {
                commandArgs.Add("EX");
                commandArgs.Add(expirySeconds.ToString());
            }

            return await ConnectAndSendAsync(async stream =>
            {
                await SendCommandAsync(stream, commandArgs.ToArray());
                var response = await ReadResponseAsync(stream);
                return response == "OK";
            });
        }

        public static async Task<bool> DelAsync(string key)
        {
            if (!_initialized)
                throw new InvalidOperationException("RedisClient has not been initialized. Call InitializeAsync first.");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty", nameof(key));

            return await ConnectAndSendAsync(async stream =>
            {
                await SendCommandAsync(stream, "DEL", key);
                var response = await ReadResponseAsync(stream);
                return IsDeleteSuccessResponse(response);
            });
        }

        private static bool IsDeleteSuccessResponse(string? response)
        {
            return response?.StartsWith(":") == true &&
                   int.TryParse(response.AsSpan(1), out int deletedCount) &&
                   deletedCount > 0;
        }

    }
}