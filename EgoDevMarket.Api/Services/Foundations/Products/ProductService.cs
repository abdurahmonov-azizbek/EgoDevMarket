// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EgoDevMarket.Api.Brokers.DateTimes;
using EgoDevMarket.Api.Brokers.Loggings;
using EgoDevMarket.Api.Brokers.Storages;
using EgoDevMarket.Api.Models.Products;

namespace EgoDevMarket.Api.Services.Foundations.Products
{
    public partial class ProductService : IProductService
    {

        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ProductService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)

        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Product> AddProductAsync(Product product) =>
        TryCatch(async () =>
        {
            ValidateProductOnAdd(product);

            return await this.storageBroker.InsertProductAsync(product);
        });

        public IQueryable<Product> RetrieveAllProducts() =>
            TryCatch(() => this.storageBroker.SelectAllProducts());

        public ValueTask<Product> RetrieveProductByIdAsync(Guid productId) =>
           TryCatch(async () =>
           {
               ValidateProductId(productId);

               Product maybeProduct =
                   await storageBroker.SelectProductByIdAsync(productId);

               ValidateStorageProduct(maybeProduct, productId);

               return maybeProduct;
           });

        public ValueTask<Product> ModifyProductAsync(Product product) =>
            TryCatch(async () =>
            {
                ValidateProductOnModify(product);

                Product maybeProduct =
                    await this.storageBroker.SelectProductByIdAsync(product.Id);

                ValidateAgainstStorageProductOnModify(inputProduct: product, storageProduct: maybeProduct);

                return await this.storageBroker.UpdateProductAsync(product);
            });

        public ValueTask<Product> RemoveProductByIdAsync(Guid productId) =>
           TryCatch(async () =>
           {
               ValidateProductId(productId);

               Product maybeProduct =
                   await this.storageBroker.SelectProductByIdAsync(productId);

               ValidateStorageProduct(maybeProduct, productId);

               return await this.storageBroker.DeleteProductAsync(maybeProduct);
           });
    }
}