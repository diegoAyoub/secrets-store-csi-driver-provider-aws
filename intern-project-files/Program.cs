// This was the program.cs file used for the .NET application, this file assumes the secret is called "my-rdb-secret"
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using MySqlConnector;

class Program
{
    static void Main(string[] args)
    {
        string directoryPath = @"C:\mnt\secrets-store";
        string fileNameNoExtension = "my-rdb-secret";

        try
        {
            // Find the secret file in the directory
            var file = Directory.EnumerateFiles(directoryPath)
                .FirstOrDefault(f => Path.GetFileNameWithoutExtension(f)
                .Equals(fileNameNoExtension, StringComparison.OrdinalIgnoreCase));

            if (file == null)
            {
                Console.WriteLine($"No secret file found in '{directoryPath}' with the name '{fileNameNoExtension}'");
                return; // Exit if the file is not found
            }

            // Read the contents of the file
            string secretJson = File.ReadAllText(file);

            // Parse the JSON content
            var secretData = JsonSerializer.Deserialize<Secret>(secretJson);

            if (secretData == null)
            {
                Console.WriteLine("Failed to deserialize the secret data.");
                return; // Exit if deserialization fails
            }

            // Create the connection string
            string connectionString = $"Server={secretData.host};User ID={secretData.username};Password={secretData.password};";

            // Connect to the database
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connection successful!");

                    // Example query
                    string query = "SELECT NOW();";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        var result = command.ExecuteScalar();
                        Console.WriteLine($"Server Time: {result}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while connecting to the database: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

    // Define the Secret class to match the JSON structure
    public class Secret
    {
        public string? username { get; set; }
        public string? password { get; set; }
        public string? host { get; set; }
    }
}