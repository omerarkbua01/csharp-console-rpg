# Console RPG — Character System

A menu-driven role-playing game for the terminal, written in **C# / .NET 8**. Create characters, give them skills, take on quests,
earn XP and level up. Progress is saved to a JSON file between runs.

It started as a learning project for core C# (classes, collections, LINQ, JSON) and grew into a small game with tested rules.

## Features

- **Characters** — name, age, class (Warrior, Mage, Archer) and three stats: strength, intelligence, endurance
- **Skills** — add, list, upgrade with skill points, remove
- **Quests** — accept from a quest board, track progress, complete for an XP reward
- **Leveling** — XP carries over, a large reward can grant several levels at once, each level grows stats by class
- **Save file** — everything is stored in `characters.json`; a damaged file is backed up instead of crashing the game,
  and saves from the first version are repaired and translated on load
- **Input validation** — every number is range-checked, so mistyped input never ends the game

## Getting started

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
git clone https://github.com/omerarkbua01/csharp-console-rpg.git
cd csharp-console-rpg
dotnet run --project src/ConsoleRpg      # play
dotnet test                              # run the unit tests
```

The save file `characters.json` is created in the folder you run the game from.

## Sample session

```text
=== CHARACTER SYSTEM ===
Active character: Aria (Level 1 Mage)
...
11 - Accept quest
12 - List quests
13 - Complete quest
Your choice: 11
=== AVAILABLE QUESTS ===
1 - Goblin Hunt (50 XP)
2 - Forest Expedition (100 XP)
3 - Castle Defense (150 XP)
4 - Back
Which quest? 3
Quest 'Castle Defense' accepted.

Your choice: 13
=== Aria's quests ===
ID: 3 - Castle Defense (in progress)
0 - Back
ID of the quest to complete: 3
Quest 'Castle Defense' completed! +150 XP - level up! Now level 2.

Your choice: 7
=== ACTIVE CHARACTER ===
Name         : Aria
Class        : Mage
Level        : 2
XP           : 64/200
Skill points : 1
Strength     : 3
Intelligence : 11
Endurance    : 6
```

## Game rules

| Rule | Value |
|---|---|
| XP needed for the next level | `100 × current level` (level 1 → 2: 100 XP, 2 → 3: 200 XP, …) |
| Warrior level-up | +2 strength, +1 endurance |
| Mage level-up | +2 intelligence, +1 endurance |
| Archer level-up | +1 strength, +1 intelligence, +1 endurance |
| Every level-up | +1 power to every skill, +1 skill point |
| New character | level 1, 1 skill point, a small random XP head start |

## Project structure

```text
src/ConsoleRpg/
  Program.cs          menu, input handling and screens
  Models.cs           Character, Skill, Quest
  Leveling.cs         XP and level-up rules (no console I/O, unit tested)
  CharacterStore.cs   JSON save/load, corrupt-file backup, old-save repair
tests/ConsoleRpg.Tests/
  LevelingTests.cs        XP thresholds, multi-level gains, class rewards
  CharacterStoreTests.cs  round trip, corrupt file, old save format
```

## What I practiced

- Object-oriented modelling with classes, properties and collections
- LINQ (`Any`, `Max`, `Find`) and `System.Text.Json` serialization
- Defensive input handling with `int.TryParse` and range checks
- Keeping game rules separate from console I/O so they can be tested
- Unit testing with xUnit (`[Fact]`, `[Theory]`, temporary files)

## History

Written between December 2025 and February 2026 while learning C#, originally with a Turkish interface.
Cleaned up and published in November 2026: English interface, .NET 8, stricter input validation, safer save-file handling,
leveling rules moved into their own class, and unit tests.

## License

[MIT](LICENSE)
