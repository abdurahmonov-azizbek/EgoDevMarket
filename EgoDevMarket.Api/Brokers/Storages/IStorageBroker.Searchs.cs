// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Searchs;

namespace EgoDevMarket.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Search> InsertSearchAsync(Search search);
        IQueryable<Search> SelectAllSearchs();
        ValueTask<Search> SelectSearchByIdAsync(Guid searchId);
        ValueTask<Search> DeleteSearchAsync(Search search);
        ValueTask<Search> UpdateSearchAsync(Search search);
    }
}