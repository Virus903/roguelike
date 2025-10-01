using System;
using System.Collections.Generic;

namespace RogueLikeGame
{
    // Класс Weapon
    public class Weapon
    {
        public string Name { get; private set; }
        public int Damage { get; private set; }
        public int Durability { get; set; }

        public Weapon(string name, int damage, int durability)
        {
            Name = name;
            Damage = damage;
            Durability = durability;
        }

        public int Use()
        {
            if (Durability > 0)
            {
                Durability--;
                return Damage;
            }
            else
            {
                Console.WriteLine($"{Name} сломалось и не может быть использовано.");
                return 0;
            }
        }
    }

    // Класс Aid
    public class Aid
    {
        public string Name { get; private set; }
        public int HealAmount { get; private set; }

        public Aid(string name, int healAmount)
        {
            Name = name;
            HealAmount = healAmount;
        }
    }

    // Класс Enemy
    public class Enemy
    {
        public string Name { get; private set; }
        public int CurrentHealth { get; private set; }
        public int MaxHealth { get; private set; }
        public Weapon Weapon { get; private set; }  // Оружие врага

        public Enemy(string name, int maxHealth, Weapon weapon)
        {
            Name = name;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            Weapon = weapon;
        }

        public int Attack()
        {
            if (Weapon != null)
            {
                return Weapon.Use();
            }
            else
            {
                return 5; // базовый урон без оружия
            }
        }

        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth < 0) CurrentHealth = 0;
        }

        public bool IsAlive()
        {
            return CurrentHealth > 0;
        }
    }

    // Класс Player
    public class Player
    {
        public string Name { get; private set; }
        public int CurrentHealth { get; private set; }
        public int MaxHealth { get; private set; }
        public int Score { get; set; }
        public Aid Medkit { get; set; }
        public List<Weapon> Inventory { get; private set; } = new List<Weapon>();

        public Player(string name, int maxHealth)
        {
            Name = name;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            Score = 0;
        }

        public void Heal()
        {
            if (Medkit != null)
            {
                CurrentHealth += Medkit.HealAmount;
                if (CurrentHealth > MaxHealth)
                    CurrentHealth = MaxHealth;
                Console.WriteLine($"{Name} использовал {Medkit.Name} и восстановил здоровье до {CurrentHealth}");
            }
        }

        public int Attack()
        {
            if (Inventory.Count == 0)
            {
                int baseDamage = 5;
                Console.WriteLine($"{Name} атакует без оружия и наносит {baseDamage} урона.");
                return baseDamage;
            }

            Console.WriteLine("Выберите оружие для атаки из инвентаря:");
            for (int i = 0; i < Inventory.Count; i++)
            {
                Weapon w = Inventory[i];
                Console.WriteLine($"{i + 1}. {w.Name} (Урон: {w.Damage}, Прочность: {w.Durability})");
            }

            int choice = 0;
            while (true)
            {
                Console.Write("Введите номер оружия: ");
                if (int.TryParse(Console.ReadLine(), out choice))
                {
                    if (choice >= 1 && choice <= Inventory.Count)
                        break;
                }
                Console.WriteLine("Некорректный ввод, попробуйте снова.");
            }

            Weapon selectedWeapon = Inventory[choice - 1];
            int damage = selectedWeapon.Use();

            Console.WriteLine($"{Name} атакует оружием {selectedWeapon.Name} и наносит {damage} урона.");
            if (selectedWeapon.Durability == 0)
            {
                Console.WriteLine($"{selectedWeapon.Name} сломалось и было удалено из инвентаря.");
                Inventory.RemoveAt(choice - 1);
            }
            return damage;
        }

        public void TakeDamage(int damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth < 0) CurrentHealth = 0;
            Console.WriteLine($"{Name} получил {damage} урона, здоровье: {CurrentHealth}");
        }

        public bool IsAlive()
        {
            return CurrentHealth > 0;
        }

        public void AddWeaponToInventory(Weapon weapon)
        {
            if (Inventory.Count < 3)
            {
                Inventory.Add(weapon);
                Console.WriteLine($"В инвентарь добавлено оружие: {weapon.Name} (Урон: {weapon.Damage}, Прочность: {weapon.Durability})");
            }
            else
            {
                Console.WriteLine("Инвентарь полон, оружие не добавлено.");
            }
        }
    }

    class Program
    {
        static readonly Random rnd = new Random();

        static List<string> enemyNames = new List<string> { "Гоблин", "Караванщик", "Наемник", "Жук-танк", "Носорог", "Зомби" };

        static List<Weapon> initialWeapons = new List<Weapon>
        {
            new Weapon("Пистолет", 15, 10),
            new Weapon("Дробовик", 20, 8),
            new Weapon("Меч", 10, 15),
            new Weapon("Лук", 12, 12),
            new Weapon("Киянка", 8, 20),
            new Weapon("Коса", 18, 10)
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Введите имя игрока:");
            string playerName = Console.ReadLine();

            Player player = new Player(playerName, 100);
            player.Medkit = new Aid("Аптечка", 20);

            // Добавляем 3 случайных оружия в инвентарь
            AddRandomWeaponsToInventory(player, 3);

            Console.WriteLine($"Добро пожаловать, {player.Name}! Начинаем игру...");

            bool continueGame = true;

            while (continueGame && player.IsAlive())
            {
                Console.WriteLine("\nНажмите Enter, чтобы продолжить или введите 'выход' для завершения:");
                string command = Console.ReadLine();
                if (command.ToLower() == "выход")
                {
                    break;
                }

                Enemy enemy = GenerateRandomEnemy();

                Console.WriteLine($"\nВы встретили врага: {enemy.Name} (Здоровье: {enemy.CurrentHealth})");

                while (player.IsAlive() && enemy.IsAlive())
                {
                    Console.WriteLine($"\nВаше здоровье: {player.CurrentHealth}");
                    Console.WriteLine($"{enemy.Name} здоровье: {enemy.CurrentHealth}");
                    Console.WriteLine("Выберите действие:\n1. Атаковать\n2. Использовать аптечку\n3. Убежать");
                    string choice = Console.ReadLine();

                    if (choice == "1")
                    {
                        int damage = player.Attack();
                        enemy.TakeDamage(damage);
                        if (!enemy.IsAlive())
                        {
                            Console.WriteLine($"Вы победили {enemy.Name}!");
                            player.Score += 10;

                            // шанс выпадения оружия
                            if (rnd.NextDouble() < 0.5)
                            {
                                Weapon droppedWeapon = GetRandomWeaponCopy();
                                Console.WriteLine($"Враг уронил оружие: {droppedWeapon.Name} (Урон: {droppedWeapon.Damage}, Прочность: {droppedWeapon.Durability})");
                                player.AddWeaponToInventory(droppedWeapon);
                            }

                            break;
                        }
                        int damageToPlayer = enemy.Attack();
                        player.TakeDamage(damageToPlayer);
                    }
                    else if (choice == "2")
                    {
                        player.Heal();
                        int damageToPlayer = enemy.Attack();
                        player.TakeDamage(damageToPlayer);
                    }
                    else if (choice == "3")
                    {
                        Console.WriteLine("Вы убежали от врага.");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Некорректный выбор, пропускаем ход.");
                    }
                }

                if (!player.IsAlive())
                {
                    Console.WriteLine("Вы погибли. Игра окончена.");
                    continueGame = false;
                }
            }

            Console.WriteLine($"Игра завершена. Ваш счет: {player.Score}");
        }

        static void AddRandomWeaponsToInventory(Player player, int count)
        {
            List<Weapon> copyWeapons = new List<Weapon>(initialWeapons);
            for (int i = 0; i < count; i++)
            {
                if (copyWeapons.Count == 0)
                    break;
                int index = rnd.Next(copyWeapons.Count);
                Weapon w = copyWeapons[index];
                copyWeapons.RemoveAt(index);
                player.AddWeaponToInventory(new Weapon(w.Name, w.Damage, w.Durability));
            }
        }

        static Weapon GetRandomWeaponCopy()
        {
            Weapon w = initialWeapons[rnd.Next(initialWeapons.Count)];
            return new Weapon(w.Name, w.Damage, w.Durability);
        }

        static Enemy GenerateRandomEnemy()
        {
            string name = enemyNames[rnd.Next(enemyNames.Count)];
            int health = rnd.Next(50, 101);
            Weapon enemyWeapon = null;
            if (rnd.NextDouble() < 0.8)
            {
                Weapon w = initialWeapons[rnd.Next(initialWeapons.Count)];
                enemyWeapon = new Weapon(w.Name, w.Damage, w.Durability);
            }
            return new Enemy(name, health, enemyWeapon);
        }
    }
}