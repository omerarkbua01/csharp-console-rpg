using ConsoleRpg;

namespace ConsoleRpg.Tests;

public class LevelingTests
{
    private static Character NewCharacter(string classType, int xp = 0) => new()
    {
        Name = "Test",
        ClassType = classType,
        Level = 1,
        Xp = xp,
        Strength = 5,
        Intelligence = 5,
        Endurance = 5,
    };

    [Theory]
    [InlineData(1, 100)]
    [InlineData(2, 200)]
    [InlineData(10, 1000)]
    public void RequiredXp_GrowsLinearlyWithLevel(int level, int expected)
    {
        Assert.Equal(expected, Leveling.RequiredXpForNextLevel(level));
    }

    [Fact]
    public void NotEnoughXp_DoesNotLevelUp()
    {
        var c = NewCharacter(CharacterClasses.Warrior, xp: 99);

        Assert.False(Leveling.TryLevelUp(c));
        Assert.Equal(1, c.Level);
        Assert.Equal(99, c.Xp);
    }

    [Fact]
    public void LeftoverXp_CarriesOverAfterLevelUp()
    {
        var c = NewCharacter(CharacterClasses.Warrior, xp: 130);

        Assert.True(Leveling.TryLevelUp(c));
        Assert.Equal(2, c.Level);
        Assert.Equal(30, c.Xp);
    }

    [Fact]
    public void BigReward_GrantsSeveralLevelsAtOnce()
    {
        // 100 (1→2) + 200 (2→3) = 300, 50 left over (level 3 needs 300 more).
        var c = NewCharacter(CharacterClasses.Mage, xp: 350);

        Leveling.TryLevelUp(c);

        Assert.Equal(3, c.Level);
        Assert.Equal(50, c.Xp);
        Assert.Equal(2, c.SkillPoints);
    }

    [Theory]
    [InlineData(CharacterClasses.Warrior, 7, 5, 6)]
    [InlineData(CharacterClasses.Mage, 5, 7, 6)]
    [InlineData(CharacterClasses.Archer, 6, 6, 6)]
    public void LevelUpRewards_DependOnClass(string classType, int strength, int intelligence, int endurance)
    {
        var c = NewCharacter(classType);

        Leveling.ApplyLevelUpRewards(c);

        Assert.Equal((strength, intelligence, endurance), (c.Strength, c.Intelligence, c.Endurance));
    }

    [Fact]
    public void LevelUp_StrengthensEverySkillAndGrantsASkillPoint()
    {
        var c = NewCharacter(CharacterClasses.Archer);
        c.Skills.Add(new Skill { SkillName = "Double Shot", Power = 10 });
        c.Skills.Add(new Skill { SkillName = "Eagle Eye", Power = 3 });

        Leveling.ApplyLevelUpRewards(c);

        Assert.Equal(new[] { 11, 4 }, c.Skills.Select(s => s.Power));
        Assert.Equal(1, c.SkillPoints);
    }
}
