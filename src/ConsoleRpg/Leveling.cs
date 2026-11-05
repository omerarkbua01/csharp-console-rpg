namespace ConsoleRpg;

/// <summary>XP and level-up rules. Kept free of console I/O so they can be unit tested.</summary>
public static class Leveling
{
    /// <summary>XP needed to go from <paramref name="currentLevel"/> to the next level (level 1 → 2 needs 100 XP).</summary>
    public static int RequiredXpForNextLevel(int currentLevel) => 100 * currentLevel;

    /// <summary>
    /// Spends XP on as many level-ups as it covers (a big reward can grant several levels at once) and applies the
    /// rewards for each one. Returns true if at least one level was gained.
    /// </summary>
    public static bool TryLevelUp(Character c)
    {
        bool leveledUp = false;
        while (c.Xp >= RequiredXpForNextLevel(c.Level))
        {
            c.Xp -= RequiredXpForNextLevel(c.Level);
            c.Level++;
            leveledUp = true;
            ApplyLevelUpRewards(c);
        }
        return leveledUp;
    }

    /// <summary>Class-specific stat growth, +1 power to every skill and +1 skill point per level.</summary>
    public static void ApplyLevelUpRewards(Character c)
    {
        switch (c.ClassType)
        {
            case CharacterClasses.Warrior:
                c.Strength += 2;
                c.Endurance += 1;
                break;
            case CharacterClasses.Mage:
                c.Intelligence += 2;
                c.Endurance += 1;
                break;
            default: // Archer and any unknown class grow evenly
                c.Strength += 1;
                c.Intelligence += 1;
                c.Endurance += 1;
                break;
        }

        foreach (var s in c.Skills)
        {
            s.Power += 1;
        }
        c.SkillPoints += 1;
    }
}
