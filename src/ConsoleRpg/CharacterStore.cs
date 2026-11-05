using System.Text.Json;

namespace ConsoleRpg;

/// <summary>Loads and saves characters as a JSON file.</summary>
public class CharacterStore
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    // Saves made by the first (Turkish) version of the app used these class names.
    private static readonly Dictionary<string, string> LegacyClassNames = new()
    {
        ["Savaşçı"] = CharacterClasses.Warrior,
        ["Büyücü"] = CharacterClasses.Mage,
        ["Okçu"] = CharacterClasses.Archer,
    };

    private readonly string _filePath;

    public CharacterStore(string filePath) => _filePath = filePath;

    /// <summary>Set when the save file could not be read and a fresh list was started; tells the user where the old file went.</summary>
    public string? LoadWarning { get; private set; }

    public List<Character> Load()
    {
        LoadWarning = null;
        if (!File.Exists(_filePath))
        {
            return new List<Character>();
        }

        List<Character>? characters;
        try
        {
            characters = JsonSerializer.Deserialize<List<Character>>(File.ReadAllText(_filePath));
        }
        catch (JsonException)
        {
            // A damaged save must not crash the game: keep a copy for inspection and start with an empty list.
            string backup = _filePath + ".corrupt";
            File.Copy(_filePath, backup, overwrite: true);
            LoadWarning = $"The save file could not be read and was copied to '{backup}'. Starting with no characters.";
            return new List<Character>();
        }

        characters ??= new List<Character>();
        foreach (var c in characters)
        {
            // Older saves may miss newer fields or contain out-of-range values.
            c.Skills ??= new List<Skill>();
            c.Quests ??= new List<Quest>();
            if (c.Level <= 0) c.Level = 1;
            if (c.Xp < 0) c.Xp = 0;
            if (LegacyClassNames.TryGetValue(c.ClassType, out var english)) c.ClassType = english;
        }
        return characters;
    }

    public void Save(List<Character> characters)
    {
        File.WriteAllText(_filePath, JsonSerializer.Serialize(characters, JsonOptions));
    }
}
