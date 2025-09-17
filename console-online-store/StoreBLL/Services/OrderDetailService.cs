namespace StoreBLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;

using StoreBLL.Interfaces;
using StoreBLL.Models;

using StoreDAL.Data;
using StoreDAL.Entities;
using StoreDAL.Interfaces;
using StoreDAL.Repository;

public class OrderDetailService : ICrud
{
    private readonly IOrderDetailRepository repository;

    public OrderDetailService(StoreDbContext context)
    {
        this.repository = new OrderDetailRepository(context);
    }

    public void Add(AbstractModel model)
    {
        if (model is not OrderDetailModel m)
        {
            throw new ArgumentException("Expected OrderDetailModel", nameof(model));
        }

        var entity = new OrderDetail(
            id: 0,
            orderId: m.OrderId,
            productId: m.ProductId,
            price: m.Price,
            amount: m.ProductAmount);
        this.repository.Add(entity);
    }

    public void Delete(int modelId)
    {
        this.repository.DeleteById(modelId);
    }

    public IEnumerable<AbstractModel> GetAll()
    {
        return this.repository.GetAll().Select(x =>
            new OrderDetailModel(
                id: x.Id,
                orderId: x.OrderId,
                productId: x.ProductId,
                price: x.Price,
                amount: x.ProductAmount));
    }

    public AbstractModel GetById(int id)
    {
        var x = this.repository.GetById(id);
        if (x == null)
        {
            return null;
        }

        return new OrderDetailModel(
            id: x.Id,
            orderId: x.OrderId,
            productId: x.ProductId,
            price: x.Price,
            amount: x.ProductAmount);
    }

    public void Update(AbstractModel model)
    {
        if (model is not OrderDetailModel m)
        {
            throw new ArgumentException("Expected OrderDetailModel", nameof(model));
        }

        var entity = this.repository.GetById(m.Id);
        if (entity == null)
        {
            return;
        }

        entity.OrderId = m.OrderId;
        entity.ProductId = m.ProductId;
        entity.Price = m.Price;
        entity.ProductAmount = m.ProductAmount;

        this.repository.Update(entity);
    }
}
