using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGD208_spring2025_CengizBugraGokakin
{
/// <summary>
/// Manages a collection of pets, their lifecycle, and stat changes in the pet simulation game.
/// </summary>
public class PetManager
{
    private List<Pet> _pets;
    private bool _isRunning;
    private readonly Random _random;

    /// <summary>
    /// Event triggered when a new pet is added.
    /// </summary>
    public event EventHandler<Pet> OnPetAdded;
    /// <summary>
    /// Event triggered when a pet is removed (dies).
    /// </summary>
    public event EventHandler<Pet> OnPetRemoved;

    /// <summary>
    /// Initializes a new instance of the PetManager class.
    /// </summary>
    public PetManager()
    {
        _pets = new List<Pet>();
        _random = new Random();
    }

    /// <summary>
    /// Adds a new pet to the manager and subscribes to its death event.
    /// </summary>
    public void AddPet(Pet pet)
    {
        _pets.Add(pet);
        pet.OnPetDied += (sender, e) => RemovePet((Pet)sender);
        OnPetAdded?.Invoke(this, pet);
    }

    /// <summary>
    /// Removes a pet from the manager and triggers the OnPetRemoved event.
    /// </summary>
    public void RemovePet(Pet pet)
    {
        _pets.Remove(pet);
        OnPetRemoved?.Invoke(this, pet);
    }

    /// <summary>
    /// Returns a list of all managed pets.
    /// </summary>
    public List<Pet> GetPets()
    {
        return _pets.ToList();
    }

    /// <summary>
    /// Starts a loop that periodically decreases a random stat for each pet.
    /// </summary>
    public async Task StartStatDecrease()
    {
        _isRunning = true;
        while (_isRunning)
        {
            foreach (var pet in _pets.ToList())
            {
                if (pet.IsAlive)
                {
                    // Randomly decrease one stat
                    var stat = (PetStat)_random.Next(0, 3);
                    pet.DecreaseStat(stat, 1);
                }
            }
            await Task.Delay(3000); // Decrease stats every 3 seconds
        }
    }

    /// <summary>
    /// Stops the stat decrease loop.
    /// </summary>
    public void StopStatDecrease()
    {
        _isRunning = false;
    }

    /// <summary>
    /// Uses an item on a pet, increasing the relevant stat if possible.
    /// </summary>
    /// <returns>True if the item was used successfully, false otherwise.</returns>
    public bool UseItem(Item item, Pet pet)
    {
        if (!pet.IsAlive) return false;
        if (!item.CompatibleWith.Contains(pet.Type)) return false;

        pet.IncreaseStat(item.AffectedStat, item.EffectAmount);
        return true;
    }
}
} 