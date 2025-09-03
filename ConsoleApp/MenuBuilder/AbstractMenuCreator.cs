using StoreDAL.Data;

namespace ConsoleMenu.Builder;
public abstract class AbstractMenuCreator : IMenuCreator
{
    public abstract (ConsoleKey id, string caption, Action action)[] GetMenuItems(StoreDbContext context);

    public Menu Create(StoreDbContext context)
    {
        return new Menu(this.GetMenuItems(context));
    }
}