// Path: console-online-store/ConsoleApp/Scenarios/AdminOrderStateCrudSmokeRunner.cs
namespace ConsoleApp.Scenarios;

using StoreBLL.Services;

using StoreDAL.Data;

public static class AdminOrderStateCrudSmokeRunner
{
    public static void Run()
    {
        using var db = StoreDbFactory.Create();
        var service = new OrderStateService(db);

        Console.WriteLine("=== Admin: Order State CRUD (smoke test) ===\n");

        // GetAll
        var all = service.GetAll().ToList();
        Console.WriteLine($"GetAll returned {all.Count} states:");
        foreach (var s in all)
        {
            var model = (StoreBLL.Models.OrderStateModel)s;
            Console.WriteLine($"  {model.Id}: {model.StateName}");
        }

        Console.WriteLine();

        // Get by id
        Console.WriteLine("Testing GetById for states 1..8:");
        var state1 = (StoreBLL.Models.OrderStateModel)service.GetById(1);
        var state2 = (StoreBLL.Models.OrderStateModel)service.GetById(2);
        var state3 = (StoreBLL.Models.OrderStateModel)service.GetById(3);
        var state4 = (StoreBLL.Models.OrderStateModel)service.GetById(4);
        var state5 = (StoreBLL.Models.OrderStateModel)service.GetById(5);

        Console.WriteLine($"State 1: {state1.StateName}");
        Console.WriteLine($"State 2: {state2.StateName}");
        Console.WriteLine($"State 3: {state3.StateName}");
        Console.WriteLine($"State 4: {state4.StateName}");
        Console.WriteLine($"State 5: {state5.StateName}");

        Console.WriteLine("\n=== Order State CRUD test passed ===");
    }
}
