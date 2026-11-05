using System.Collections.Generic;

namespace Program
{
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

        public List<Skill> Skills { get; set; } = new List<Skill>();
        public List<Quest> Quests { get; set; } = new List<Quest>();
    }
}
   
