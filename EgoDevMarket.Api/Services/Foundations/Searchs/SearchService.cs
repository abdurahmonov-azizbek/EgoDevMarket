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
using EgoDevMarket.Api.Models.Searchs;

namespace EgoDevMarket.Api.Services.Foundations.Searchs
{
    public partial class SearchService : ISearchService
    {

        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public SearchService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)

        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Search> AddSearchAsync(Search search) =>
        TryCatch(async () =>
        {
            ValidateSearchOnAdd(search);

            return await this.storageBroker.InsertSearchAsync(search);
        });

        public IQueryable<Search> RetrieveAllSearchs() =>
            TryCatch(() => this.storageBroker.SelectAllSearchs());

        public ValueTask<Search> RetrieveSearchByIdAsync(Guid searchId) =>
           TryCatch(async () =>
           {
               ValidateSearchId(searchId);

               Search maybeSearch =
                   await storageBroker.SelectSearchByIdAsync(searchId);

               ValidateStorageSearch(maybeSearch, searchId);

               return maybeSearch;
           });

        public ValueTask<Search> ModifySearchAsync(Search search) =>
            TryCatch(async () =>
            {
                ValidateSearchOnModify(search);

                Search maybeSearch =
                    await this.storageBroker.SelectSearchByIdAsync(search.Id);

                ValidateAgainstStorageSearchOnModify(inputSearch: search, storageSearch: maybeSearch);

                return await this.storageBroker.UpdateSearchAsync(search);
            });

        public ValueTask<Search> RemoveSearchByIdAsync(Guid searchId) =>
           TryCatch(async () =>
           {
               ValidateSearchId(searchId);

               Search maybeSearch =
                   await this.storageBroker.SelectSearchByIdAsync(searchId);

               ValidateStorageSearch(maybeSearch, searchId);

               return await this.storageBroker.DeleteSearchAsync(maybeSearch);
           });
    }
}