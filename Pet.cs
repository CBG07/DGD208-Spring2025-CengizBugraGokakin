using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DGD208_spring2025_CengizBugraGokakin
{
/// <summary>
/// Represents a virtual pet with stats and events for a pet simulation game.
/// </summary>
public class Pet
{
    /// <summary>
    /// The name of the pet.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The type/species of the pet.
    /// </summary>
    public PetType Type { get; set; }
    /// <summary>
    /// The stats of the pet (Hunger, Sleep, Fun).
    /// </summary>
    public Dictionary<PetStat, int> Stats { get; private set; }
    /// <summary>
    /// Indicates whether the pet is alive.
    /// </summary>
    public bool IsAlive { get; private set; }

    /// <summary>
    /// Event triggered when a pet stat changes.
    /// </summary>
    public event EventHandler<PetStat> OnStatChanged;
    /// <summary>
    /// Event triggered when the pet dies.
    /// </summary>
    public event EventHandler OnPetDied;

    /// <summary>
    /// Creates a new pet with the given name and type.
    /// </summary>
    public Pet(string name, PetType type)
    {
        Name = name;
        Type = type;
        IsAlive = true;
        InitializeStats();
    }

    /// <summary>
    /// Initializes the pet's stats to default values.
    /// </summary>
    private void InitializeStats()
    {
        Stats = new Dictionary<PetStat, int>
        {
            { PetStat.Hunger, 50 },
            { PetStat.Sleep, 50 },
            { PetStat.Fun, 50 }
        };
    }

    /// <summary>
    /// Decreases the specified stat by a given amount. If the stat reaches 0, the pet dies.
    /// </summary>
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

    /// <summary>
    /// Increases the specified stat by a given amount, up to a maximum of 100.
    /// </summary>
    public void IncreaseStat(PetStat stat, int amount)
    {
        if (!IsAlive) return;

        Stats[stat] = Math.Min(100, Stats[stat] + amount);
        OnStatChanged?.Invoke(this, stat);
    }

    /// <summary>
    /// Handles the pet's death and triggers the OnPetDied event.
    /// </summary>
    private void Die()
    {
        IsAlive = false;
        OnPetDied?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Returns a string representation of the pet and its stats.
    /// </summary>
    public override string ToString()
    {
        return $"{Name} ({Type}) - Hunger: {Stats[PetStat.Hunger]}, Sleep: {Stats[PetStat.Sleep]}, Fun: {Stats[PetStat.Fun]}";
    }
}
} 