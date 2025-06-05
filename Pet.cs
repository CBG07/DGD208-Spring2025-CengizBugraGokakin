using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DGD208_spring2025_CengizBugraGokakin
{
public class Pet
{
    public string Name { get; set; }
    public PetType Type { get; set; }
    public Dictionary<PetStat, int> Stats { get; private set; }
    public bool IsAlive { get; private set; }

    public event EventHandler<PetStat> OnStatChanged;
    public event EventHandler OnPetDied;

    public Pet(string name, PetType type)
    {
        Name = name;
        Type = type;
        IsAlive = true;
        InitializeStats();
    }

    private void InitializeStats()
    {
        Stats = new Dictionary<PetStat, int>
        {
            { PetStat.Hunger, 50 },
            { PetStat.Sleep, 50 },
            { PetStat.Fun, 50 }
        };
    }

    public void DecreaseStat(PetStat stat, int amount)
    {
        if (!IsAlive) return;

        Stats[stat] = Math.Max(0, Stats[stat] - amount);
        OnStatChanged?.Invoke(this, stat);

        if (Stats[stat] == 0)
        {
            Die();
        }
    }

    public void IncreaseStat(PetStat stat, int amount)
    {
        if (!IsAlive) return;

        Stats[stat] = Math.Min(100, Stats[stat] + amount);
        OnStatChanged?.Invoke(this, stat);
    }

    private void Die()
    {
        IsAlive = false;
        OnPetDied?.Invoke(this, EventArgs.Empty);
    }

    public override string ToString()
    {
        return $"{Name} ({Type}) - Hunger: {Stats[PetStat.Hunger]}, Sleep: {Stats[PetStat.Sleep]}, Fun: {Stats[PetStat.Fun]}";
    }
}
} 