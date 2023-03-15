namespace YVendingMachine;

public class VendingMachine
{
    private Wallet _wallet;

    public List<Product> Products { get; } = new();
    
    public void Start(Wallet w)
    {
        _wallet = w;
        
        while (true)
        {
            Console.Clear();
            ShowProductsMenu();
            
            var choice = ReadChoice("Please Select Your choice: ");
        
            if(choice == Products.Count + 1)
                return;
            
            if (choice <= 0)
            {
                Console.WriteLine("Invalid choice. Press any key to continue...");
                Console.ReadKey(true);
                continue;
            }

            var product = Products[choice - 1];
            Process(product);
        }
    }

    public void DoBuy(Product p)
    {
        
    }
    
    void Process(Product p)
    {
        ShowProductPage(p);
        if (!ConfirmBuy())
            return;
        
        Buy(p);
        p.Use();

        Console.Write("Press any key to continue...");
        Console.ReadKey(true);
    }
    
    void ShowProductsMenu()

    {

        Console.WriteLine("***********************************************");
        Console.WriteLine("*                  WELCOME                    *");
        Console.WriteLine("*                    TO                       *");
        Console.WriteLine("*          YUNUS VENDING MACHINE              *");
        Console.WriteLine("*                                             *");
        Console.WriteLine("*      Please Select from the Menu Below      *");
        Console.WriteLine("*         And Pay with Notes/Coins As         *");
        Console.WriteLine("*                    Follows:                 *");
        Console.WriteLine("*                    1kr = 10 st              *");
        Console.WriteLine("*                    5kr = 10 st              *");
        Console.WriteLine("*                   10kr = 10 st              *");
        Console.WriteLine("***********************************************");

        for (int i = 0; i < Products.Count; i++)
            Console.WriteLine($"{i + 1}. {Products[i].Name}");
        Console.WriteLine("===============================================");
        Console.WriteLine($"{Products.Count + 1}. Quit");
        Console.WriteLine();
    }

    void ShowProductPage(Product p)
    {
        Console.Clear();
        Console.WriteLine(p.Name);
        Console.WriteLine("===================================");
        Console.WriteLine(p.Description());
        Console.WriteLine($"Price: {p.Price}kr");
        Console.WriteLine();
    }
    
    static int ReadChoice(string message)
    {
        Console.Write(message);
        var input = Console.ReadLine();
        
        if (!int.TryParse(input, out var number))
            return -1;

        return number;
    }

    static bool ConfirmBuy()
    {
        Console.WriteLine("1. Buy");
        Console.WriteLine("2. Go Back");
        Console.WriteLine();
        var choice  = ReadChoice("Your Choice: ");
        if (choice == 1)
            return true;

        if (choice != 2)
        {
            Console.WriteLine("Invalid choice. Press any key to continue...");
            Console.ReadKey(true);
        }

        return false;
    }

    void Buy(Product p)
    {
        var userPaid = SetInMoney();
        var amount = userPaid - p.Price;
        
        while (p.Price > userPaid)
        {
            Console.WriteLine($"You're short in {-amount}kr");
            if (!ConfirmContinuePaying())
            {
                _wallet.PutIn(userPaid);
                return;
            }
                
            userPaid += SetInMoney(userPaid);
        }
        
        ReturnMoney(amount);
        ShowMessageAndWaitForAKey("Purchase successful");
        
        
        Console.Clear();
    }
    
    int SetInMoney(int amount = 0)
    {
        while (true)
        {
            ShowMoneyMenu(amount);

            var denomination = ReadChoice("Denomination: ");
            if (denomination == -1)
                break;
        
            if (Array.IndexOf(Wallet.Denominations, denomination) < 0)
            {
                ShowMessageAndWaitForAKey("Invalid denomination");
                continue;
            }
            
            var count = ReadChoice("How many? ");
            
            if (count < 0)
            {
                ShowMessageAndWaitForAKey("Invalid amount");
                continue;
            }

            if (count > _wallet.Count(denomination))
            {
                ShowMessageAndWaitForAKey("Your balance is not enough");
                continue;
            }
            
            _wallet.TakeOut(denomination, count);
            amount += denomination * count;
        }
        
        return amount;
    }

    private static void ShowMessageAndWaitForAKey(string message)
    {
        Console.WriteLine($"{message}. Press any key to continue...");
        Console.ReadKey(true);
    }

    void ReturnMoney(int amount)
    {
        if (amount > 0)
            Console.WriteLine($"You got {amount}kr back");
        
        foreach (var denomination in Wallet.Denominations)
        {
            _wallet.PutIn(denomination, amount / denomination);
            amount %= denomination;
        }
    }
    
    void ShowMoneyMenu(int amount)
    {
        Console.Clear();
        Console.WriteLine("Set in money");
        Console.WriteLine();
        Console.WriteLine($"Total: {amount}kr");
    }

    bool ConfirmContinuePaying()
    {
        Console.Write("Want to continue paying? [y/n] ");
        while (true)
        {
            var answer = Console.ReadKey(true).KeyChar;
            
            if (!"yn".Contains(answer))
                continue;

            Console.WriteLine(answer);
            return answer == 'y';
        }
    }
}
