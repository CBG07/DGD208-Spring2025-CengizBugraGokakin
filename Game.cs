using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGD208_spring2025_CengizBugraGokakin
{
public class Game
{
    private bool _isRunning; // Controls whether the game loop is running
    private readonly PetManager _petManager; // Class that manages pets

    public Game()
    {
        _petManager = new PetManager();
        
        // Subscribe to the event triggered when a pet is added
        _petManager.OnPetAdded += (sender, pet) => Console.WriteLine($"New pet added: {pet}");
        
        // Subscribe to the event triggered when a pet dies
        _petManager.OnPetRemoved += async (sender, pet) =>
        {
            Console.WriteLine($"Pet has died: {pet}");
            await Task.Delay(2000); // 2 seconds wait
        };
    }

    public async Task GameLoop()
    {
        // Start the game
        Initialize();

        Console.WriteLine("\nPress any key to start...");
        Console.ReadKey();
        
        // Start decreasing pet statistics
        _ = _petManager.StartStatDecrease();
        
        // Main game loop
        _isRunning = true;
        while (_isRunning)
        {
            // Show menu and get user selection
            string userChoice = GetUserInput();
            
            // Process user selection
            await ProcessUserChoice(userChoice);
        }
        
        // Stop decreasing statistics when game ends
        _petManager.StopStatDecrease();
        
        Console.WriteLine("Thanks for playing!");
    }

    private void Initialize()
    {
        // Initial message
        Console.WriteLine("Welcome to Pet Simulator!");
        Console.WriteLine("Created by: [Cengiz Bugra Gokakin] - [2305041076]");
    }

    private string GetUserInput()
    {
        // Main menu options
        var menuItems = new List<string>
        {
            "1. Adopt a new pet",
            "2. View all pets",
            "3. Use an item",
            "4. Exit game"
        };
        
        // Create menu object and get user selection
        var menu = new Menu<string>("Main Menu", menuItems, item => item);
        var selection = menu.ShowAndGetSelection();
        if (selection == null) return "";
        return selection[0].ToString(); // Return selected menu number
    }

    private async Task ProcessUserChoice(string choice)
    {
        // Execute operations based on user selection
        switch (choice)
        {
            case "1":
                await AdoptPet(); // Adopt a pet
                break;
            case "2":
                ViewPets(); // View current pets
                break;
            case "3":
                await UseItem(); // Use item
                break;
            case "4":
                _isRunning = false; // Exit game
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again."); // Invalid input
                break;
        }
    }

    private async Task AdoptPet()
    {
        // Select pet type
        Console.WriteLine("\nChoose a pet type:");
        var petTypes = Enum.GetValues(typeof(PetType)).Cast<PetType>().ToList();
        var petTypeMenu = petTypes.Select((pt, i) => $"{i + 1}. {pt}").ToList();
        var menu = new Menu<string>("Pet Types", petTypeMenu, item => item);
        var selection = menu.ShowAndGetSelection();
        if (selection == null) return;

        // If selection is valid, get name and create pet
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
        // Display all pets
        var pets = _petManager.GetPets();
        if (pets.Count == 0)
        {
            Console.WriteLine("You don't have any pets yet!");
        }
        else
        {
            Console.WriteLine("\nYour pets:");
            foreach (var pet in pets)
            {
                Console.WriteLine(pet); // Print pet information
            }
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadLine();
    }

    private async Task UseItem()
    {
        var pets = _petManager.GetPets();
        if (pets.Count == 0)
        {
            Console.WriteLine("You don't have any pets to use items on!");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return;
        }

        // Let user select a pet
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

        // Filter items suitable for the selected pet
        var availableItems = ItemDatabase.AllItems.Where(i => i.CompatibleWith.Contains(selectedPet.Type)).ToList();
        if (availableItems.Count == 0)
        {
            Console.WriteLine("No items available for this pet type!");
            return;
        }

        // Ask user to select an item
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

        // Use item
        Console.WriteLine($"Using {selectedItem.Name} on {selectedPet.Name}...");
        if (_petManager.UseItem(selectedItem, selectedPet))
        {
            await Task.Delay((int)(selectedItem.Duration * 1000)); // Wait for item effect duration
            Console.WriteLine($"Successfully used {selectedItem.Name} on {selectedPet.Name}!");
        }
        else
        {
            Console.WriteLine("Failed to use the item.");
        }
    }
}
}
