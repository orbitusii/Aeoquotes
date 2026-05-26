using System.Text.Json;
using log4net;

namespace Aeoquotes;

public static class Migrator
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(Migrator));
    public static void OldJsonToEF(string jsonPath, QuotesContext dbContext)
    {
        if (!dbContext.Quotes.Any() && File.Exists(jsonPath))
        {
            dbContext.Database.EnsureDeleted();
            
            bool created = dbContext.Database.EnsureCreated();
            Logger.Debug($"Database EnsureCreated returned: {created}");

            if (!File.Exists(jsonPath))
            {
                Logger.Error($"Error: Could not find the source JSON file at: {jsonPath}");
                return;
            }

            string json = File.ReadAllText(jsonPath);
            var items = JsonSerializer.Deserialize<List<Quote>>(json);

            if (items == null || items.Count == 0)
            {
                Logger.Error("Error: JSON was parsed but returned 0 items. Check your JSON structure or class property names.");
                return;
            }

            Logger.Info($"Found {items.Count} items in JSON. Adding to database...");

            dbContext.Quotes.AddRange(items);
            
            int rowsWritten = dbContext.SaveChanges();
            Logger.Info($"Migration complete! Saved {rowsWritten} rows to the database.");
        }
    }
}