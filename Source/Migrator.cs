using System.Text.Json;

namespace Aeoquotes;

public static class Migrator
{
    public static void OldJsonToEF(string jsonPath, QuotesContext dbContext)
    {
        if (!dbContext.Quotes.Any() && File.Exists(jsonPath))
        {
            dbContext.Database.EnsureDeleted();
            
            bool created = dbContext.Database.EnsureCreated();
            Logging.Log($"Database EnsureCreated returned: {created}");

            if (!File.Exists(jsonPath))
            {
                Logging.Log($"Error: Could not find the source JSON file at: {jsonPath}");
                return;
            }

            string json = File.ReadAllText(jsonPath);
            var items = JsonSerializer.Deserialize<List<Quote>>(json);

            if (items == null || items.Count == 0)
            {
                Logging.Log("Error: JSON was parsed but returned 0 items. Check your JSON structure or class property names.");
                return;
            }

            Logging.Log($"Found {items.Count} items in JSON. Adding to database...");

            dbContext.Quotes.AddRange(items);
            
            int rowsWritten = dbContext.SaveChanges();
            Logging.Log($"Migration complete! Saved {rowsWritten} rows to the database.");
        }
    }
}