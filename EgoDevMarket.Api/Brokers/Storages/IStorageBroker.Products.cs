// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Products;

namespace EgoDevMarket.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Product> InsertProductAsync(Product product);
        IQueryable<Product> SelectAllProducts();
        ValueTask<Product> SelectProductByIdAsync(Guid productId);
        ValueTask<Product> DeleteProductAsync(Product product);
        ValueTask<Product> UpdateProductAsync(Product product);
    }
}