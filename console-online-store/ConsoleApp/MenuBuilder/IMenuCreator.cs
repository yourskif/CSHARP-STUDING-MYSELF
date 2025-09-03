/*
Yuriy Antonov copyright 2018-2020
*/
using StoreDAL.Data;

namespace ConsoleMenu.Builder;

public interface IMenuCreator
{
    Menu Create(StoreDbContext context);
}