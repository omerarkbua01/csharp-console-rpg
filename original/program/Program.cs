using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
namespace Program
{

    class Program
    {
        static List<Character> characters = new List<Character>();
        static int nextId = 1;
        static Character activeCharacter = null;
        static string filepath = "characters.json";


        static void Main(string[] args)
        {
            LoadCharacters();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== KARAKTER SİSTEMİ ===");
                Console.WriteLine("0-Çıkış");
                Console.WriteLine("1-Karakter Oluştur ");
                Console.WriteLine("2-Karakterleri Listele");
                Console.WriteLine("3-Karakter Sil");
                Console.WriteLine("4-Skill Ekle");
                Console.WriteLine("5-Skill Listele");
                Console.WriteLine("6-Karakter Seç");
                Console.WriteLine("7-Aktif Karakter Bilgisi");
                Console.WriteLine("8-XP Ekle");
                Console.WriteLine("9-Skill Upgrade");
                Console.WriteLine("10-Skill Sil");
                Console.WriteLine("11-Quest Al");
                Console.WriteLine("12-Quest Listele");
                Console.WriteLine("13-Quest Tamamla");
                Console.Write("Seçimin : ");
                string secim = Console.ReadLine();

                if (secim == "0")
                {
                    break;
                }
                else if (secim == "1")
                {
                    CreateCharacter();
                }
                else if (secim == "2")
                {
                    ListCharacter();
                }
                else if (secim == "3")
                {
                    DeleteCharacter();
                }
                else if (secim == "4")
                {
                    AddSkill();
                }
                else if (secim == "5")
                {
                    ListSkills();
                }
                else if (secim == "6")
                {
                    SelectCharacter();
                }
                else if (secim == "7")
                {
                    ShowActiveCharacter();
                }
                else if (secim == "8")
                {
                    AddXpToActiveCharacter();
                }
                else if (secim=="9")
                {
                    UpgradeSkill();
                }
                else if (secim == "10")
                {
                    RemoveSkill();
                }
                else if (secim == "11")
                {
                    AddQuestToActive();
                }
                else if (secim == "12")
                {
                    ListQuests();
                }
                else if (secim == "13")
                {
                    CompleteQuest();
                }
                else
                {
                    Console.WriteLine("Yanlış Tuşlama");
                    Console.ReadLine();
                }
            }
        }



        static void CreateCharacter()
        {
            while (true)
            {
                Console.Write("Karakter Adı : ");
                string name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("İsim boş olamaz!");
                    continue;
                }

                Console.Write("Yaş : ");
                if (!int.TryParse(Console.ReadLine(), out int age))
                {
                    Console.WriteLine("Geçerli bir sayı gir.");
                    continue;
                }

                while (true)
                {
                    Console.WriteLine("1-Savaşçı");
                    Console.WriteLine("2-Büyücü");
                    Console.WriteLine("3-Okçu");
                    Console.Write(" Sınıf seç : ");
                    if (!int.TryParse(Console.ReadLine(), out int classChoice) || classChoice < 1 || classChoice > 3)
                    {
                        Console.WriteLine("Geçerli bir sayı gir.");
                        Console.ReadKey();
                        continue;
                    }
                    Console.Write("Güç (1-10): ");
                    if (!int.TryParse(Console.ReadLine(), out int str) || str < 1 || str > 10)
                    {
                        Console.WriteLine("Geçerli bir sayı gir.");
                        Console.ReadKey();
                        continue;
                    }
                    Console.Write("Zeka (1-10): ");
                    if (!int.TryParse(Console.ReadLine(), out int intel) || intel < 1 || intel > 10)
                    {
                        Console.WriteLine("Geçerli bir sayı gir.");
                        Console.ReadKey();
                        continue;
                    }
                    Console.Write("Dayanıklılık (1-10): ");
                    if (!int.TryParse(Console.ReadLine(), out int endu) || endu < 1 || endu > 10)
                    {
                        Console.WriteLine("Geçerli bir sayı gir.");
                        Console.ReadKey();
                        continue;
                    }

                    string classType = "";
                    if (classChoice == 1) classType = "Savaşçı";
                    else if (classChoice == 2) classType = "Büyücü";
                    else if (classChoice == 3) classType = "Okçu";
                    else classType = "Bilinmiyor";


                    Character c = new Character();
                    c.Id = nextId++;
                    c.Name = name;
                    c.Age = age;
                    c.Strength = str;
                    c.Intelligence = intel;
                    c.Endurance = endu;
                    c.ClassType = classType;
                    c.Level = 1;
                    c.Xp = Random.Shared.Next(1,20);
                    c.SkillPoints = 1;
                    characters.Add(c);
                    SaveCharacters();
                    Console.WriteLine("Karakter Oluşturuldu ! ");
                    Console.ReadKey();
                    return;
                }

            }
        }


        static void ListCharacter()
        {
            Console.Clear();
            Console.WriteLine("=== KAYITLI KARAKTERLER ===");
            if (characters.Count == 0)
            {
                Console.WriteLine("Hiç karakter Yok . ");
            }
            else
            {
                foreach (var c in characters)
                {
                    Console.WriteLine($"ID           : {c.Id}");
                    Console.WriteLine($"İsim         : {c.Name}");
                    Console.WriteLine($"Yaş          : {c.Age}");
                    Console.WriteLine($"Level        : {c.Level}");
                    Console.WriteLine($"Aktif Xp     : {c.Xp}");
                    Console.WriteLine($"Sınıf        : {c.ClassType}");
                    Console.WriteLine($"Güç          : {c.Strength}");
                    Console.WriteLine($"Zeka         : {c.Intelligence}");
                    Console.WriteLine($"Dayanıklılık : {c.Endurance}");
                    Console.WriteLine("========================");
                }
            }
            Console.ReadKey();

        }



        static void DeleteCharacter()
        {
            Console.Write("Silmek istediğin karakter ID'si : ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Geçerli bir sayı gir.");
                Console.ReadKey();
                return;
            }
            var character = characters.Find(c => c.Id == id);
            if (character == null)
            {
                Console.WriteLine("Bu ID'de karakater bulunamadı..");
                Console.ReadKey();
                return;
            }
            if (activeCharacter == character)
                activeCharacter = null;

            characters.Remove(character);
            SaveCharacters();
            Console.WriteLine("Karakter Silindi!");
            Console.ReadKey();
        }



        static void AddSkill()
        {
            while (true)
            {
                if (activeCharacter == null)
                {
                    Console.WriteLine("Önce bir karakter seçmelisin! (Menü 6)");
                    Console.ReadKey();
                    return;
                }
                Console.Write($"{activeCharacter.Name} karakterine eklenecek Skill Adı : ");
                string name = Console.ReadLine();

                Console.Write("Skill Gücü : ");
                if (!int.TryParse(Console.ReadLine(), out int power))
                {
                    Console.WriteLine("Geçerli bir sayı gir.");
                    Console.ReadKey();
                    continue;
                }

                Skill s = new Skill { SkillName = name, Power = power };
                activeCharacter.Skills.Add(s);

                SaveCharacters();

                Console.WriteLine("Skill Başarıyla Eklendi!");
                Console.ReadKey();
                return;
            }
        }



        static void ListSkills()
        {
            Console.Write("Skillerini görmek istediğin karakter ID : ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Geçerli bir sayı gir.");
                Console.ReadKey();
                return;
            }

            var character = characters.Find(c => c.Id == id);

            if (character == null)
            {
                Console.WriteLine("Karakter Bulunamadı!");
                Console.ReadKey();
                return;
            }
            Console.WriteLine($"=== {character.Name} Skill Listesi ===");

            if (character.Skills.Count == 0)
            {
                Console.WriteLine("Bu karakterde skill yok.");
            }
            else
            {
                foreach (var s in character.Skills)
                {
                    Console.WriteLine($"Skill : {s.SkillName} - Güç : {s.Power}");
                }
            }
            Console.ReadKey();
        }


        static void SelectCharacter()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(" === KARAKTER SEÇME ===");

                if (characters.Count == 0)
                {
                    Console.WriteLine("Hiç Karakter yok. Önce Karakter oluşturun.");
                    Console.ReadKey();
                    return;
                }
                foreach (var c in characters)
                {
                    Console.WriteLine($"ID: {c.Id} - {c.Name} ({c.ClassType})");
                }
                Console.Write("Seçmek istediğin karakter ID : ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Geçerli bir sayı gir.");
                    Console.ReadKey();
                    continue;
                }
                var selected = characters.Find(c => c.Id == id);

                if (selected == null)
                {
                    Console.WriteLine("Böyle bir ID yok.");
                    Console.ReadKey();
                    continue;
                }
                activeCharacter = selected;
                Console.WriteLine($"Aktif Karakterler: {activeCharacter.Name}");
                Console.ReadKey();
                return;
            }

        }


        static void ShowActiveCharacter()
        {
            Console.Clear();
            if (activeCharacter == null)
            {
                Console.WriteLine("henüz bir karakter seçilmedi(Önce bir karakter seçin");
                Console.ReadKey();
                return;
            }
            Console.WriteLine(" === AKTİF KARAKTER BİLGİLERİ === ");
            Console.WriteLine($"ID               : {activeCharacter.Id}");
            Console.WriteLine($"İsim             : {activeCharacter.Name}");
            Console.WriteLine($"Yaş              : {activeCharacter.Age}");
            Console.WriteLine($"Sınıf            : {activeCharacter.ClassType}");
            Console.WriteLine($"Level            : {activeCharacter.Level}");
            Console.WriteLine($"XP               : {activeCharacter.Xp}");
            Console.WriteLine($"Skill Point      : {activeCharacter.SkillPoints}");
            Console.WriteLine($"Güç              : {activeCharacter.Strength}");
            Console.WriteLine($"Zeka             : {activeCharacter.Intelligence}");
            Console.WriteLine($"Dayanıklılık     : {activeCharacter.Endurance}");
            Console.WriteLine();
            Console.ReadKey();

        }

            static void AddXpToActiveCharacter()
            {
                if (activeCharacter == null)
                {
                    Console.WriteLine("Önce bir karakter seçmelisin (Menü 6)");
                    Console.ReadKey();
                    return;
                }
                Console.Write("Kaç XP eklemek istiyorsun? : ");
                if (!int.TryParse(Console.ReadLine(), out int xp) || xp <= 0)
                {
                    Console.WriteLine("Geçerli bir sayı gir.");
                    Console.ReadKey();
                    return;
                }
                activeCharacter.Xp += xp;

                bool leveled = TryLevelUp(activeCharacter);
                SaveCharacters();
                Console.WriteLine(leveled
                    ? "XP EKLENDİ ve LEVEL ATLADIN"
                    : "XP EKLENDİ !");
                Console.ReadKey();
            }

        static void UpgradeSkill()
        {
            Console.Clear();
            if (activeCharacter == null)
            {
                Console.WriteLine("Önce bir karakter seçmelisin! (Menü 6)");
                Console.ReadKey();
                return;
            }
            if (activeCharacter.Skills == null || activeCharacter.Skills.Count == 0)
            {
                Console.WriteLine("Skill yok!");
                Console.ReadKey();
                return;
            }
            Console.WriteLine($"=== {activeCharacter.Name} - Skill Upgrade ===");
            Console.WriteLine($" Karakterin Sahip olduğu Skill Puanı : {activeCharacter.SkillPoints}");
            int index = 1;
            foreach (var s in activeCharacter.Skills)
            {
                Console.WriteLine($"{index} - {s.SkillName} (Güç : {s.Power})");
                index++;
            }
            Console.WriteLine($"{activeCharacter.Skills.Count + 1}- Geri Dön  ");
            Console.Write("Hangi Skill'i upgrade edeceksin : ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > activeCharacter.Skills.Count + 1)
            {
                Console.WriteLine("Geçerli bir sayı girin.");
                Console.ReadKey();
                return;
            }
            if (choice == activeCharacter.Skills.Count + 1)
            {
                return;
            }
            Console.Write("Kaç puan basacaksın : ");
            if(!int.TryParse(Console.ReadLine(), out int points))
            {
                Console.WriteLine("Geçerli birsayı girin.");
                Console.ReadKey();
                return;
            }
            if(points <= 0)
            {
                Console.WriteLine("Puan 0'dan büyük olmalı");
                Console.ReadKey();
                return;
            }
            if(points>activeCharacter.SkillPoints)
            {
                Console.WriteLine($"Yeterli puan yok! Mevcut : {activeCharacter.SkillPoints}");
                Console.ReadKey();
                return;
            }
            Skill selectedSkill = activeCharacter.Skills[choice - 1];
            selectedSkill.Power += points;
            activeCharacter.SkillPoints -= points;
            SaveCharacters();
            Console.WriteLine($"{selectedSkill.SkillName} başarıyla güçlendirildi! (+{points} güç)");
            Console.ReadKey();

        }

        static void RemoveSkill()
        {
            Console.Clear();
            if (activeCharacter == null)
            {
                Console.WriteLine("Önce bir karakter seçmelisin! (Menü 6)");
                Console.ReadKey();
                return;
            }
            if(activeCharacter.Skills == null || activeCharacter.Skills.Count == 0)
            {
                Console.WriteLine("Silinecek Skill Yok!");
                Console.ReadKey();
                return;
            }
            Console.WriteLine($"=== {activeCharacter.Name} - Skill Silme ===");
            int index = 1;
            foreach(var s in activeCharacter.Skills)
            {
                Console.WriteLine($" {index} - {s.SkillName} (Güç : {s.Power})");
                index++;
            }
            Console.WriteLine($"{activeCharacter.Skills.Count + 1}- Geri Dön");
            Console.Write("Hangi Skill'i Sileceksin : ");
            if (!int.TryParse(Console.ReadLine(), out int choice) || choice < 1 || choice > activeCharacter.Skills.Count + 1)
            {
                Console.WriteLine("Geçerli bir sayı girin.");
                Console.ReadKey();
                return;
            }
            if (choice == activeCharacter.Skills.Count + 1)
            {
                return;
            }
            activeCharacter.Skills.RemoveAt(choice - 1);
            SaveCharacters();
            Console.WriteLine("Skill Başarıyla silindi!");
            Console.ReadKey();
        }

        static void AddQuestToActive()
        {
            Console.Clear();
            if (activeCharacter == null)
            {
                Console.WriteLine("Aktif Karakter Yok. Karakter seçin (MENÜ 6)");
                Console.ReadKey();
                return;
            }
            List<Quest> availableQuests = new List<Quest>
           {
               new Quest{Id= 1, Title="Goblin Avı", XpReward=50, IsCompleted=false},
               new Quest{Id= 2, Title="Orman Keşfi", XpReward=100, IsCompleted=false},
               new Quest{Id= 3, Title="Kale Savunması", XpReward=150, IsCompleted=false}
           };
            Console.WriteLine("=== Mevcut Görevler ===");
            int index = 1;
            foreach(var quest in availableQuests)
            {
                Console.WriteLine($"{index} - {quest.Title} (XP : {quest.XpReward})");
                index++;
            }
            Console.WriteLine($"{availableQuests.Count + 1} - Geri Dön");
            Console.Write("Hangi görevi alacaksın : ");
            if (!int.TryParse(Console.ReadLine(),out int choice) || choice < 1 || choice > availableQuests.Count + 1)
            {
                Console.WriteLine("Hatalı sayı tekrar deneyin.");
                Console.ReadKey();
                return;
            }
            if(choice == availableQuests.Count + 1)
            {
                return;
            }

            Quest selectedQuest = availableQuests[choice - 1];
            bool alreadyHas = activeCharacter.Quests.Any(q => q.Id == selectedQuest.Id);
            if (alreadyHas)
            {
                Console.WriteLine("Bu görevi zaten aldın!");
                Console.ReadKey();
                return;
            }
            Quest newQuest = new Quest
            {
                Id = selectedQuest.Id,
                Title = selectedQuest.Title,
                XpReward = selectedQuest.XpReward,
                IsCompleted = false
            };
            activeCharacter.Quests.Add(newQuest);
            SaveCharacters();
            Console.WriteLine($"' {newQuest.Title} ' görevi alındı!");
            Console.ReadKey();
        }

        static void ListQuests()
        {
            Console.Clear();
            if (activeCharacter == null)
            {
                Console.WriteLine("Aktif Karakter Yok. Karakter seçin (MENÜ 6)");
                Console.ReadKey();
                return;
            }
            if(activeCharacter.Quests == null || activeCharacter.Quests.Count==0)
            {
                Console.WriteLine("Henü görev almadınız , Görev alın : (MENÜ 11)");
                Console.ReadKey();
                return;
            }
            Console.WriteLine($"=== {activeCharacter.Name} - Quest Listesi ===");
            foreach (var quest in activeCharacter.Quests)
            {
                string status = quest.IsCompleted ? "✅ Tamamlandı" : "⏳ Devam Ediyor";
                Console.WriteLine($"ID: {quest.Id} - {quest.Title} - XP: {quest.XpReward} - Durum: {status}");
            }
            Console.ReadKey();
        }

        static void CompleteQuest()
        {
            Console.Clear();
            if (activeCharacter == null)
            {
                Console.WriteLine("Aktif Karakter Yok. Karakter Seçin (MENÜ 6)");
                Console.ReadKey();
                return;
            }
            if (activeCharacter.Quests == null || activeCharacter.Quests.Count == 0)
            {
                Console.WriteLine("Henüz görev almadınız , Görev alın : (MENÜ 11)");
                Console.ReadKey();
                return;
            }
            Console.WriteLine($"=== {activeCharacter.Name} - Görevler ===");
            foreach(var q in activeCharacter.Quests)
            {
                string durum = q.IsCompleted ? "✅" : "⏳";
                Console.WriteLine($"ID : {q.Id} - {q.Title} ({durum})");
            }
            Console.WriteLine("0 - Geri");
            Console.Write("Tamamlamak istediğin Görev ID : ");
            if (!int.TryParse(Console.ReadLine(), out int secim1))
            {
                Console.WriteLine("Geçerli bir sayı girin.");
                Console.ReadKey();
                return;
            }
            if (secim1 == 0)
            {
                return;
            }
            var quest = activeCharacter.Quests.Find(q => q.Id == secim1);
            if (quest == null)
            {
                Console.WriteLine("Bu ID'de quest bulunamadı..");
                Console.ReadKey();
                return;
            }
            if (quest.IsCompleted)
            {
                Console.WriteLine("Bu quest zaten tamamlanmış!");
                Console.ReadKey();
                return;
            }
            quest.IsCompleted = true;
            activeCharacter.Xp += quest.XpReward;
            TryLevelUp(activeCharacter);
            SaveCharacters();
            Console.WriteLine($"' {quest.Title} ' tamamlandı! + {quest.XpReward} XP kazandın!");
            Console.ReadKey();
        }

        static bool TryLevelUp(Character c)
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
            static int RequiredXpForNextLevel(int currentLevel)
            {
                return 100 * currentLevel;
            }
            static void ApplyLevelUpRewards(Character c)
            {
                if (c.ClassType == "Savaşçı")
                {
                    c.Strength += 2;
                    c.Endurance += 1;
                }
                else if (c.ClassType == "Büyücü")
                {
                    c.Intelligence += 2;
                    c.Endurance += 1;
                }
                else if (c.ClassType == "Okçu")
                {
                    c.Strength += 1;
                    c.Intelligence += 1;
                    c.Endurance += 1;
                }
                else
                {
                    c.Strength += 1;
                    c.Intelligence += 1;
                    c.Endurance += 1;
                }
                if (c.Skills != null)
                {
                    foreach (var s in c.Skills)
                    {
                        s.Power += 1;
                    }
                }
            c.SkillPoints += 1;
        }


        static void SaveCharacters()
            {
                var json = JsonSerializer.Serialize(characters);
                File.WriteAllText(filepath, json);
            }



            static void LoadCharacters()
            {
                if (!File.Exists(filepath))
                {
                    characters = new List<Character>();
                    nextId = 1;
                    return;
                }

      

            var json = File.ReadAllText(filepath);

                characters = JsonSerializer.Deserialize<List<Character>>(json);

                if (characters == null)
                    characters = new List<Character>();

                // Skills null ise düzelt
                foreach (var c in characters)
                {
                    if (c.Skills == null)
                        c.Skills = new List<Skill>();
                    if (c.Level <= 0)
                        c.Level = 1; // Eski Kayıtlar için
                    if (c.Xp < 0)
                        c.Xp = 0;
                }

                // nextId düzelt
                if (characters.Count > 0)
                    nextId = characters.Max(x => x.Id) + 1;
                else
                    nextId = 1;
            }








    }
}
