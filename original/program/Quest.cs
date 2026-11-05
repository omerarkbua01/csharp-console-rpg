using System;

namespace Program
{
    public class Quest
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public int XpReward { get; set; }
        public bool IsCompleted { get; set; }
    }
}

