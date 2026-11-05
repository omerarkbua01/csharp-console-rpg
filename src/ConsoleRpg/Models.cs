namespace ConsoleRpg;

/// <summary>A playable character. Property names match the JSON save file, so older saves keep loading.</summary>
public class Character
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Age { get; set; }
    public string ClassType { get; set; } = "";

    public int Strength { get; set; }
    public int Intelligence { get; set; }
    public int Endurance { get; set; }
    public int Level { get; set; }
    public int Xp { get; set; }
    public int SkillPoints { get; set; }

    public List<Skill> Skills { get; set; } = new();
    public List<Quest> Quests { get; set; } = new();
}

public class Skill
{
    public string SkillName { get; set; } = "";
    public int Power { get; set; }
}

public class Quest
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int XpReward { get; set; }
    public bool IsCompleted { get; set; }
}

public static class CharacterClasses
{
    public const string Warrior = "Warrior";
    public const string Mage = "Mage";
    public const string Archer = "Archer";

    public static readonly string[] All = { Warrior, Mage, Archer };
}
