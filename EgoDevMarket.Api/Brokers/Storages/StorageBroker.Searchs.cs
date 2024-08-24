// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Searchs;
using Microsoft.EntityFrameworkCore;

namespace EgoDevMarket.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Search> Searchs { get; set; }

        public async ValueTask<Search> InsertSearchAsync(Search search) =>
            await InsertAsync(search);

        public IQueryable<Search> SelectAllSearchs() =>
            SelectAll<Search>();

        public async ValueTask<Search> SelectSearchByIdAsync(Guid searchId) =>
            await SelectAsync<Search>(searchId);

        public async ValueTask<Search> DeleteSearchAsync(Search search) =>
            await DeleteAsync(search);

        public async ValueTask<Search> UpdateSearchAsync(Search search) =>
            await UpdateAsync(search);
    }
}