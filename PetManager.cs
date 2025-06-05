using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGD208_spring2025_CengizBugraGokakin
{
public class PetManager
{
    private List<Pet> _pets;
    private bool _isRunning;
    private readonly Random _random;

    public event EventHandler<Pet> OnPetAdded;
    public event EventHandler<Pet> OnPetRemoved;

    public PetManager()
    {
        _pets = new List<Pet>();
        _random = new Random();
    }

    public void AddPet(Pet pet)
    {
        _pets.Add(pet);
        pet.OnPetDied += (sender, e) => RemovePet((Pet)sender);
        OnPetAdded?.Invoke(this, pet);
    }

    public void RemovePet(Pet pet)
    {
        _pets.Remove(pet);
        OnPetRemoved?.Invoke(this, pet);
    }

    public List<Pet> GetPets()
    {
        return _pets.ToList();
    }

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

    public void StopStatDecrease()
    {
        _isRunning = false;
    }

    public bool UseItem(Item item, Pet pet)
    {
        if (!pet.IsAlive) return false;
        if (!item.CompatibleWith.Contains(pet.Type)) return false;

        pet.IncreaseStat(item.AffectedStat, item.EffectAmount);
        return true;
    }
}
} 