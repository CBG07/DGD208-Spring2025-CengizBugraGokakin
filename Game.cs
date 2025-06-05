using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGD208_spring2025_CengizBugraGokakin
{
public class Game
{
    private bool _isRunning;
    private readonly PetManager _petManager;
    
    public Game()
    {
        _petManager = new PetManager();
        
        // Subscribe to pet events
        _petManager.OnPetAdded += (sender, pet) => Console.WriteLine($"New pet added: {pet}");
        _petManager.OnPetRemoved += (sender, pet) => Console.WriteLine($"Pet has died: {pet}");
    }
    
    public async Task GameLoop()
    {
        // Initialize the game
        Initialize();
        
        // Start the stat decrease loop
        _ = _petManager.StartStatDecrease();
        
        // Main game loop
        _isRunning = true;
        while (_isRunning)
        {
            // Display menu and get player input
            string userChoice = GetUserInput();
            
            // Process the player's choice
            await ProcessUserChoice(userChoice);
        }
        
        // Stop the stat decrease loop
        _petManager.StopStatDecrease();
        
        Console.WriteLine("Thanks for playing!");
    }
    
    private void Initialize()
    {
        Console.WriteLine("Welcome to Pet Simulator!");
        Console.WriteLine("Created by: [Your Name] - [Your Student Number]");
    }
    
    private string GetUserInput()
    {
        var menuItems = new List<string>
        {
            "1. Adopt a new pet",
            "2. View all pets",
            "3. Use an item",
            "4. Exit game"
        };
        var menu = new Menu<string>("Main Menu", menuItems, item => item);
        var selection = menu.ShowAndGetSelection();
        if (selection == null) return "";
        return selection[0].ToString(); // The first character is the menu number
    }
    
    private async Task ProcessUserChoice(string choice)
    {
        switch (choice)
        {
            case "1":
                await AdoptPet();
                break;
            case "2":
                ViewPets();
                break;
            case "3":
                await UseItem();
                break;
            case "4":
                _isRunning = false;
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }
    }
    
    private async Task AdoptPet()
    {
        Console.WriteLine("\nChoose a pet type:");
        var petTypes = Enum.GetValues(typeof(PetType)).Cast<PetType>().ToList();
        var petTypeMenu = petTypes.Select((pt, i) => $"{i + 1}. {pt}").ToList();
        var menu = new Menu<string>("Pet Types", petTypeMenu, item => item);
        var selection = menu.ShowAndGetSelection();
        if (selection == null) return;
        if (int.TryParse(selection[0].ToString(), out int petChoice) && petChoice > 0 && petChoice <= petTypes.Count)
        {
            Console.Write("Enter pet name: ");
            string name = Console.ReadLine();
            
            var pet = new Pet(name, petTypes[petChoice - 1]);
            _petManager.AddPet(pet);
            Console.WriteLine($"You adopted a {pet.Type} named {pet.Name}!");
        }
        else
        {
            Console.WriteLine("Invalid choice.");
        }
    }
    
    private void ViewPets()
    {
        var pets = _petManager.GetPets();
        if (pets.Count == 0)
        {
            Console.WriteLine("You don't have any pets yet!");
            return;
        }
        
        Console.WriteLine("\nYour pets:");
        foreach (var pet in pets)
        {
            Console.WriteLine(pet);
        }
    }
    
    private async Task UseItem()
    {
        var pets = _petManager.GetPets();
        if (pets.Count == 0)
        {
            Console.WriteLine("You don't have any pets to use items on!");
            return;
        }
        
        // Show pet selection menu
        Console.WriteLine("\nSelect a pet:");
        var petMenu = pets.Select((p, i) => $"{i + 1}. {p}").ToList();
        var petMenuObj = new Menu<string>("Select Pet", petMenu, item => item);
        var petSelection = petMenuObj.ShowAndGetSelection();
        if (petSelection == null) return;
        if (!int.TryParse(petSelection[0].ToString(), out int petChoice) || petChoice < 1 || petChoice > pets.Count)
        {
            Console.WriteLine("Invalid pet choice.");
            return;
        }
        var selectedPet = pets[petChoice - 1];
        
        // Show available items for the selected pet
        var availableItems = ItemDatabase.AllItems.Where(i => i.CompatibleWith.Contains(selectedPet.Type)).ToList();
        if (availableItems.Count == 0)
        {
            Console.WriteLine("No items available for this pet type!");
            return;
        }
        var itemMenu = availableItems.Select((i, idx) => $"{idx + 1}. {i.Name}").ToList();
        var itemMenuObj = new Menu<string>("Select Item", itemMenu, item => item);
        var itemSelection = itemMenuObj.ShowAndGetSelection();
        if (itemSelection == null) return;
        if (!int.TryParse(itemSelection[0].ToString(), out int itemChoice) || itemChoice < 1 || itemChoice > availableItems.Count)
        {
            Console.WriteLine("Invalid item choice.");
            return;
        }
        var selectedItem = availableItems[itemChoice - 1];
        
        // Use the item
        Console.WriteLine($"Using {selectedItem.Name} on {selectedPet.Name}...");
        if (_petManager.UseItem(selectedItem, selectedPet))
        {
            await Task.Delay((int)(selectedItem.Duration * 1000));
            Console.WriteLine($"Successfully used {selectedItem.Name} on {selectedPet.Name}!");
        }
        else
        {
            Console.WriteLine("Failed to use the item.");
        }
    }
}
}