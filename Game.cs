using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGD208_spring2025_CengizBugraGokakin
{
public class Game
{
    private bool _isRunning; // Oyun döngüsünün çalışıp çalışmadığını kontrol eder
    private readonly PetManager _petManager; // Evcil hayvanları yöneten sınıf

    public Game()
    {
        _petManager = new PetManager();
        
        // Evcil hayvan eklendiğinde tetiklenen olaya abone ol
        _petManager.OnPetAdded += (sender, pet) => Console.WriteLine($"New pet added: {pet}");
        
        // Evcil hayvan öldüğünde tetiklenen olaya abone ol
        _petManager.OnPetRemoved += async (sender, pet) =>
        {
            Console.WriteLine($"Pet has died: {pet}");
            await Task.Delay(2000); // 2 saniye bekle
        };
    }

    public async Task GameLoop()
    {
        // Oyunu başlat
        Initialize();

        Console.WriteLine("\nPress any key to start...");
        Console.ReadKey();
        
        // Evcil hayvanların istatistiklerini azaltmaya başla
        _ = _petManager.StartStatDecrease();
        
        // Ana oyun döngüsü
        _isRunning = true;
        while (_isRunning)
        {
            // Menü göster ve kullanıcıdan seçim al
            string userChoice = GetUserInput();
            
            // Kullanıcı seçimini işle
            await ProcessUserChoice(userChoice);
        }
        
        // Oyun bittiğinde istatistik azaltmayı durdur
        _petManager.StopStatDecrease();
        
        Console.WriteLine("Thanks for playing!");
    }

    private void Initialize()
    {
        // Başlangıç mesajı
        Console.WriteLine("Welcome to Pet Simulator!");
        Console.WriteLine("Created by: [Cengiz Buğra Gökakın] - [2305041076]");
    }

    private string GetUserInput()
    {
        // Ana menü seçenekleri
        var menuItems = new List<string>
        {
            "1. Adopt a new pet",
            "2. View all pets",
            "3. Use an item",
            "4. Exit game"
        };
        
        // Menü nesnesi oluştur ve kullanıcı seçimini al
        var menu = new Menu<string>("Main Menu", menuItems, item => item);
        var selection = menu.ShowAndGetSelection();
        if (selection == null) return "";
        return selection[0].ToString(); // Seçilen menü numarasını döndür
    }

    private async Task ProcessUserChoice(string choice)
    {
        // Kullanıcının seçimine göre işlemleri gerçekleştir
        switch (choice)
        {
            case "1":
                await AdoptPet(); // Evcil hayvan sahiplen
                break;
            case "2":
                ViewPets(); // Mevcut evcil hayvanları görüntüle
                break;
            case "3":
                await UseItem(); // Eşya kullan
                break;
            case "4":
                _isRunning = false; // Oyunu bitir
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again."); // Hatalı giriş
                break;
        }
    }

    private async Task AdoptPet()
    {
        // Evcil hayvan türünü seç
        Console.WriteLine("\nChoose a pet type:");
        var petTypes = Enum.GetValues(typeof(PetType)).Cast<PetType>().ToList();
        var petTypeMenu = petTypes.Select((pt, i) => $"{i + 1}. {pt}").ToList();
        var menu = new Menu<string>("Pet Types", petTypeMenu, item => item);
        var selection = menu.ShowAndGetSelection();
        if (selection == null) return;

        // Seçim geçerliyse isim al ve hayvanı oluştur
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
        // Tüm evcil hayvanları görüntüle
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
                Console.WriteLine(pet); // Pet bilgilerini yazdır
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

        // Kullanıcının bir evcil hayvan seçmesini sağla
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

        // Seçilen evcil hayvana uygun olan eşyaları filtrele
        var availableItems = ItemDatabase.AllItems.Where(i => i.CompatibleWith.Contains(selectedPet.Type)).ToList();
        if (availableItems.Count == 0)
        {
            Console.WriteLine("No items available for this pet type!");
            return;
        }

        // Kullanıcıdan eşya seçmesini iste
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

        // Eşyayı uygula
        Console.WriteLine($"Using {selectedItem.Name} on {selectedPet.Name}...");
        if (_petManager.UseItem(selectedItem, selectedPet))
        {
            await Task.Delay((int)(selectedItem.Duration * 1000)); // Eşya etkisi süresince bekle
            Console.WriteLine($"Successfully used {selectedItem.Name} on {selectedPet.Name}!");
        }
        else
        {
            Console.WriteLine("Failed to use the item.");
        }
    }
}
}
