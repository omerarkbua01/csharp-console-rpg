namespace ConsoleRpg;

/// <summary>Menu-driven console game: create characters, manage skills, take and complete quests, gain XP and levels.</summary>
public static class Program
{
    private static readonly CharacterStore Store = new("characters.json");
    private static List<Character> characters = new();
    private static int nextId = 1;
    private static Character? activeCharacter;

    // Quests on offer; a character keeps its own copy when it accepts one.
    private static readonly Quest[] AvailableQuests =
    {
        new() { Id = 1, Title = "Goblin Hunt", XpReward = 50 },
        new() { Id = 2, Title = "Forest Expedition", XpReward = 100 },
        new() { Id = 3, Title = "Castle Defense", XpReward = 150 },
    };

    public static void Main()
    {
        LoadCharacters();
        while (true)
        {
            ClearScreen();
            Console.WriteLine("=== CHARACTER SYSTEM ===");
            if (activeCharacter != null)
            {
                Console.WriteLine($"Active character: {activeCharacter.Name} (Level {activeCharacter.Level} {activeCharacter.ClassType})");
            }
            Console.WriteLine("0  - Exit");
            Console.WriteLine("1  - Create character");
            Console.WriteLine("2  - List characters");
            Console.WriteLine("3  - Delete character");
            Console.WriteLine("4  - Add skill");
            Console.WriteLine("5  - List skills");
            Console.WriteLine("6  - Select character");
            Console.WriteLine("7  - Active character details");
            Console.WriteLine("8  - Add XP");
            Console.WriteLine("9  - Upgrade skill");
            Console.WriteLine("10 - Remove skill");
            Console.WriteLine("11 - Accept quest");
            Console.WriteLine("12 - List quests");
            Console.WriteLine("13 - Complete quest");
            Console.Write("Your choice: ");
            string? choice = Console.ReadLine();
            if (choice == null)
            {
                return; // input ended (e.g. piped input ran out)
            }

            switch (choice.Trim())
            {
                case "0": return;
                case "1": CreateCharacter(); break;
                case "2": ListCharacters(); break;
                case "3": DeleteCharacter(); break;
                case "4": AddSkill(); break;
                case "5": ListSkills(); break;
                case "6": SelectCharacter(); break;
                case "7": ShowActiveCharacter(); break;
                case "8": AddXpToActiveCharacter(); break;
                case "9": UpgradeSkill(); break;
                case "10": RemoveSkill(); break;
                case "11": AcceptQuest(); break;
                case "12": ListQuests(); break;
                case "13": CompleteQuest(); break;
                default:
                    Console.WriteLine("Invalid choice.");
                    Pause();
                    break;
            }
        }
    }

    // ── Console helpers: safe when input/output is redirected (scripted runs, tests) ──────────────────────────
    private static void ClearScreen()
    {
        if (!Console.IsOutputRedirected) Console.Clear();
    }

    private static void Pause()
    {
        if (Console.IsInputRedirected) Console.ReadLine();
        else Console.ReadKey(intercept: true);
    }

    private static string ReadText(string prompt)
    {
        Console.Write(prompt);
        return (Console.ReadLine() ?? "").Trim();
    }

    /// <summary>Reads a whole number in [min, max]; returns null (after telling the user) when the input is not valid.</summary>
    private static int? ReadNumber(string prompt, int min, int max)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine(), out int value) && value >= min && value <= max)
        {
            return value;
        }
        Console.WriteLine($"Please enter a number between {min} and {max}.");
        return null;
    }

    private static bool RequireActiveCharacter()
    {
        if (activeCharacter != null) return true;
        Console.WriteLine("No active character. Select one first (menu 6).");
        Pause();
        return false;
    }

    // ── Characters ────────────────────────────────────────────────────────────────────────────────────────────
    private static void CreateCharacter()
    {
        string name;
        while (true)
        {
            name = ReadText("Character name: ");
            if (name.Length > 0) break;
            Console.WriteLine("Name cannot be empty.");
        }

        int? age;
        while ((age = ReadNumber("Age (1-120): ", 1, 120)) == null) { }

        Console.WriteLine("1 - Warrior");
        Console.WriteLine("2 - Mage");
        Console.WriteLine("3 - Archer");
        int? classChoice;
        while ((classChoice = ReadNumber("Choose a class: ", 1, 3)) == null) { }

        int? strength, intelligence, endurance;
        while ((strength = ReadNumber("Strength (1-10): ", 1, 10)) == null) { }
        while ((intelligence = ReadNumber("Intelligence (1-10): ", 1, 10)) == null) { }
        while ((endurance = ReadNumber("Endurance (1-10): ", 1, 10)) == null) { }

        var c = new Character
        {
            Id = nextId++,
            Name = name,
            Age = age.Value,
            ClassType = CharacterClasses.All[classChoice.Value - 1],
            Strength = strength.Value,
            Intelligence = intelligence.Value,
            Endurance = endurance.Value,
            Level = 1,
            Xp = Random.Shared.Next(1, 20), // a small random head start
            SkillPoints = 1,
        };
        characters.Add(c);
        SaveCharacters();
        Console.WriteLine($"Character '{c.Name}' created (ID {c.Id}).");
        Pause();
    }

    private static void ListCharacters()
    {
        ClearScreen();
        Console.WriteLine("=== SAVED CHARACTERS ===");
        if (characters.Count == 0)
        {
            Console.WriteLine("No characters yet.");
        }
        foreach (var c in characters)
        {
            Console.WriteLine($"ID           : {c.Id}");
            Console.WriteLine($"Name         : {c.Name}");
            Console.WriteLine($"Age          : {c.Age}");
            Console.WriteLine($"Level        : {c.Level}");
            Console.WriteLine($"XP           : {c.Xp}/{Leveling.RequiredXpForNextLevel(c.Level)}");
            Console.WriteLine($"Class        : {c.ClassType}");
            Console.WriteLine($"Strength     : {c.Strength}");
            Console.WriteLine($"Intelligence : {c.Intelligence}");
            Console.WriteLine($"Endurance    : {c.Endurance}");
            Console.WriteLine("========================");
        }
        Pause();
    }

    private static void DeleteCharacter()
    {
        int? id = ReadNumber("ID of the character to delete: ", 1, int.MaxValue);
        var character = id == null ? null : characters.Find(c => c.Id == id);
        if (character == null)
        {
            if (id != null) Console.WriteLine("No character with that ID.");
            Pause();
            return;
        }
        if (activeCharacter == character) activeCharacter = null;
        characters.Remove(character);
        SaveCharacters();
        Console.WriteLine($"Character '{character.Name}' deleted.");
        Pause();
    }

    private static void SelectCharacter()
    {
        ClearScreen();
        Console.WriteLine("=== SELECT CHARACTER ===");
        if (characters.Count == 0)
        {
            Console.WriteLine("No characters yet. Create one first (menu 1).");
            Pause();
            return;
        }
        foreach (var c in characters)
        {
            Console.WriteLine($"ID: {c.Id} - {c.Name} ({c.ClassType})");
        }
        int? id = ReadNumber("ID of the character to select: ", 1, int.MaxValue);
        var selected = id == null ? null : characters.Find(c => c.Id == id);
        if (selected == null)
        {
            if (id != null) Console.WriteLine("No character with that ID.");
            Pause();
            return;
        }
        activeCharacter = selected;
        Console.WriteLine($"Active character: {activeCharacter.Name}");
        Pause();
    }

    private static void ShowActiveCharacter()
    {
        ClearScreen();
        if (!RequireActiveCharacter()) return;
        var c = activeCharacter!;
        Console.WriteLine("=== ACTIVE CHARACTER ===");
        Console.WriteLine($"ID           : {c.Id}");
        Console.WriteLine($"Name         : {c.Name}");
        Console.WriteLine($"Age          : {c.Age}");
        Console.WriteLine($"Class        : {c.ClassType}");
        Console.WriteLine($"Level        : {c.Level}");
        Console.WriteLine($"XP           : {c.Xp}/{Leveling.RequiredXpForNextLevel(c.Level)}");
        Console.WriteLine($"Skill points : {c.SkillPoints}");
        Console.WriteLine($"Strength     : {c.Strength}");
        Console.WriteLine($"Intelligence : {c.Intelligence}");
        Console.WriteLine($"Endurance    : {c.Endurance}");
        Pause();
    }

    private static void AddXpToActiveCharacter()
    {
        if (!RequireActiveCharacter()) return;
        int? xp = ReadNumber("How much XP to add? ", 1, 100_000);
        if (xp == null)
        {
            Pause();
            return;
        }
        activeCharacter!.Xp += xp.Value;
        bool leveled = Leveling.TryLevelUp(activeCharacter);
        SaveCharacters();
        Console.WriteLine(leveled ? $"XP added - level up! Now level {activeCharacter.Level}." : "XP added.");
        Pause();
    }

    // ── Skills ────────────────────────────────────────────────────────────────────────────────────────────────
    private static void AddSkill()
    {
        if (!RequireActiveCharacter()) return;
        string name = ReadText($"Skill name for {activeCharacter!.Name}: ");
        if (name.Length == 0)
        {
            Console.WriteLine("Skill name cannot be empty.");
            Pause();
            return;
        }
        int? power = ReadNumber("Skill power (1-100): ", 1, 100);
        if (power == null)
        {
            Pause();
            return;
        }
        activeCharacter.Skills.Add(new Skill { SkillName = name, Power = power.Value });
        SaveCharacters();
        Console.WriteLine($"Skill '{name}' added.");
        Pause();
    }

    private static void ListSkills()
    {
        int? id = ReadNumber("ID of the character: ", 1, int.MaxValue);
        var character = id == null ? null : characters.Find(c => c.Id == id);
        if (character == null)
        {
            if (id != null) Console.WriteLine("No character with that ID.");
            Pause();
            return;
        }
        Console.WriteLine($"=== {character.Name}'s skills ===");
        if (character.Skills.Count == 0)
        {
            Console.WriteLine("This character has no skills.");
        }
        foreach (var s in character.Skills)
        {
            Console.WriteLine($"{s.SkillName} - power {s.Power}");
        }
        Pause();
    }

    /// <summary>Prints the active character's skills numbered from 1 plus a "Back" entry; returns the chosen index or null.</summary>
    private static int? ChooseSkill(string title, string prompt)
    {
        var skills = activeCharacter!.Skills;
        Console.WriteLine(title);
        for (int i = 0; i < skills.Count; i++)
        {
            Console.WriteLine($"{i + 1} - {skills[i].SkillName} (power {skills[i].Power})");
        }
        Console.WriteLine($"{skills.Count + 1} - Back");
        int? choice = ReadNumber(prompt, 1, skills.Count + 1);
        if (choice == null) Pause();
        return choice == null || choice == skills.Count + 1 ? null : choice - 1;
    }

    private static void UpgradeSkill()
    {
        ClearScreen();
        if (!RequireActiveCharacter()) return;
        var c = activeCharacter!;
        if (c.Skills.Count == 0)
        {
            Console.WriteLine("No skills to upgrade.");
            Pause();
            return;
        }
        int? index = ChooseSkill($"=== {c.Name} - Upgrade skill ===\nAvailable skill points: {c.SkillPoints}", "Which skill? ");
        if (index == null) return;
        if (c.SkillPoints == 0)
        {
            Console.WriteLine("No skill points left. Gain a level to earn more.");
            Pause();
            return;
        }
        int? points = ReadNumber($"How many points to spend (1-{c.SkillPoints})? ", 1, c.SkillPoints);
        if (points == null)
        {
            Pause();
            return;
        }
        var skill = c.Skills[index.Value];
        skill.Power += points.Value;
        c.SkillPoints -= points.Value;
        SaveCharacters();
        Console.WriteLine($"{skill.SkillName} upgraded (+{points} power).");
        Pause();
    }

    private static void RemoveSkill()
    {
        ClearScreen();
        if (!RequireActiveCharacter()) return;
        var c = activeCharacter!;
        if (c.Skills.Count == 0)
        {
            Console.WriteLine("No skills to remove.");
            Pause();
            return;
        }
        int? index = ChooseSkill($"=== {c.Name} - Remove skill ===", "Which skill? ");
        if (index == null) return;
        string removed = c.Skills[index.Value].SkillName;
        c.Skills.RemoveAt(index.Value);
        SaveCharacters();
        Console.WriteLine($"Skill '{removed}' removed.");
        Pause();
    }

    // ── Quests ────────────────────────────────────────────────────────────────────────────────────────────────
    private static void AcceptQuest()
    {
        ClearScreen();
        if (!RequireActiveCharacter()) return;
        Console.WriteLine("=== AVAILABLE QUESTS ===");
        for (int i = 0; i < AvailableQuests.Length; i++)
        {
            Console.WriteLine($"{i + 1} - {AvailableQuests[i].Title} ({AvailableQuests[i].XpReward} XP)");
        }
        Console.WriteLine($"{AvailableQuests.Length + 1} - Back");
        int? choice = ReadNumber("Which quest? ", 1, AvailableQuests.Length + 1);
        if (choice == null)
        {
            Pause();
            return;
        }
        if (choice == AvailableQuests.Length + 1) return;

        var offer = AvailableQuests[choice.Value - 1];
        if (activeCharacter!.Quests.Any(q => q.Id == offer.Id))
        {
            Console.WriteLine("You already have this quest.");
            Pause();
            return;
        }
        activeCharacter.Quests.Add(new Quest { Id = offer.Id, Title = offer.Title, XpReward = offer.XpReward });
        SaveCharacters();
        Console.WriteLine($"Quest '{offer.Title}' accepted.");
        Pause();
    }

    private static void ListQuests()
    {
        ClearScreen();
        if (!RequireActiveCharacter()) return;
        var c = activeCharacter!;
        if (c.Quests.Count == 0)
        {
            Console.WriteLine("No quests yet. Accept one first (menu 11).");
            Pause();
            return;
        }
        Console.WriteLine($"=== {c.Name}'s quests ===");
        foreach (var q in c.Quests)
        {
            Console.WriteLine($"ID: {q.Id} - {q.Title} - {q.XpReward} XP - {(q.IsCompleted ? "completed" : "in progress")}");
        }
        Pause();
    }

    private static void CompleteQuest()
    {
        ClearScreen();
        if (!RequireActiveCharacter()) return;
        var c = activeCharacter!;
        if (c.Quests.Count == 0)
        {
            Console.WriteLine("No quests yet. Accept one first (menu 11).");
            Pause();
            return;
        }
        Console.WriteLine($"=== {c.Name}'s quests ===");
        foreach (var q in c.Quests)
        {
            Console.WriteLine($"ID: {q.Id} - {q.Title} ({(q.IsCompleted ? "completed" : "in progress")})");
        }
        Console.WriteLine("0 - Back");
        int? id = ReadNumber("ID of the quest to complete: ", 0, int.MaxValue);
        if (id == null)
        {
            Pause();
            return;
        }
        if (id == 0) return;

        var quest = c.Quests.Find(q => q.Id == id);
        if (quest == null)
        {
            Console.WriteLine("No quest with that ID.");
        }
        else if (quest.IsCompleted)
        {
            Console.WriteLine("This quest is already completed.");
        }
        else
        {
            quest.IsCompleted = true;
            c.Xp += quest.XpReward;
            bool leveled = Leveling.TryLevelUp(c);
            SaveCharacters();
            Console.WriteLine($"Quest '{quest.Title}' completed! +{quest.XpReward} XP" + (leveled ? $" - level up! Now level {c.Level}." : ""));
        }
        Pause();
    }

    // ── Save file ─────────────────────────────────────────────────────────────────────────────────────────────
    private static void SaveCharacters() => Store.Save(characters);

    private static void LoadCharacters()
    {
        characters = Store.Load();
        nextId = characters.Count > 0 ? characters.Max(x => x.Id) + 1 : 1;
        if (Store.LoadWarning != null)
        {
            Console.WriteLine(Store.LoadWarning);
            Pause();
        }
    }
}
