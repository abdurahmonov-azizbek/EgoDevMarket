// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Products;
using Microsoft.EntityFrameworkCore;

namespace EgoDevMarket.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Product> Products { get; set; }

        public async ValueTask<Product> InsertProductAsync(Product product) =>
            await InsertAsync(product);

        public IQueryable<Product> SelectAllProducts() =>
            SelectAll<Product>();

        public async ValueTask<Product> SelectProductByIdAsync(Guid productId) =>
            await SelectAsync<Product>(productId);

        public async ValueTask<Product> DeleteProductAsync(Product product) =>
            await DeleteAsync(product);

        public async ValueTask<Product> UpdateProductAsync(Product product) =>
            await UpdateAsync(product);
    }
}