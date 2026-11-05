using ConsoleRpg;

namespace ConsoleRpg.Tests;

public sealed class CharacterStoreTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), "console-rpg-tests-" + Guid.NewGuid().ToString("N"));
    private string SavePath => Path.Combine(_dir, "characters.json");

    public CharacterStoreTests() => Directory.CreateDirectory(_dir);

    public void Dispose() => Directory.Delete(_dir, recursive: true);

    [Fact]
    public void MissingFile_StartsEmpty()
    {
        var store = new CharacterStore(SavePath);

        Assert.Empty(store.Load());
        Assert.Null(store.LoadWarning);
    }

    [Fact]
    public void SaveThenLoad_RoundTripsCharacters()
    {
        var store = new CharacterStore(SavePath);
        var hero = new Character { Id = 1, Name = "Aria", ClassType = CharacterClasses.Mage, Level = 3, Xp = 40, SkillPoints = 2 };
        hero.Skills.Add(new Skill { SkillName = "Fireball", Power = 12 });
        hero.Quests.Add(new Quest { Id = 2, Title = "Forest Expedition", XpReward = 100, IsCompleted = true });

        store.Save(new List<Character> { hero });
        var loaded = Assert.Single(store.Load());

        Assert.Equal(("Aria", 3, 40, 2), (loaded.Name, loaded.Level, loaded.Xp, loaded.SkillPoints));
        Assert.Equal("Fireball", Assert.Single(loaded.Skills).SkillName);
        Assert.True(Assert.Single(loaded.Quests).IsCompleted);
    }

    [Fact]
    public void CorruptFile_DoesNotCrash_AndKeepsABackup()
    {
        File.WriteAllText(SavePath, "{ this is not valid json");
        var store = new CharacterStore(SavePath);

        Assert.Empty(store.Load());
        Assert.NotNull(store.LoadWarning);
        Assert.Equal("{ this is not valid json", File.ReadAllText(SavePath + ".corrupt"));
    }

    [Fact]
    public void OldSaveFile_IsRepairedAndTranslated()
    {
        // Shape written by the first (Turkish) version: no Quests field, a non-positive level, Turkish class name.
        File.WriteAllText(SavePath, """[{"Id":4,"Name":"Kara","ClassType":"Savaşçı","Level":0,"Xp":-5,"Skills":null}]""");
        var store = new CharacterStore(SavePath);

        var c = Assert.Single(store.Load());

        Assert.Equal(CharacterClasses.Warrior, c.ClassType);
        Assert.Equal((1, 0), (c.Level, c.Xp));
        Assert.NotNull(c.Skills);
        Assert.NotNull(c.Quests);
    }
}
