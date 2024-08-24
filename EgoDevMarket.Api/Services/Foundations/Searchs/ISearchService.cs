// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Searchs;

namespace EgoDevMarket.Api.Services.Foundations.Searchs
{
    public interface ISearchService  
    {
        /// <exception cref="Models.Searchs.Exceptions.SearchValidationException"></exception>
        /// <exception cref="Models.Searchs.Exceptions.SearchDependencyValidationException"></exception>
        /// <exception cref="Models.Searchs.Exceptions.SearchDependencyException"></exception>
        /// <exception cref="Models.Searchs.Exceptions.SearchServiceException"></exception>
        ValueTask<Search> AddSearchAsync(Search search);

        /// <exception cref="Models.Searchs.Exceptions.SearchDependencyException"></exception>
        /// <exception cref="Models.Searchs.Exceptions.SearchServiceException"></exception>
        IQueryable<Search> RetrieveAllSearchs();

        /// <exception cref="Models.Searchs.Exceptions.SearchDependencyException"></exception>
        /// <exception cref="Models.Searchs.Exceptions.SearchServiceException"></exception>
        ValueTask<Search> RetrieveSearchByIdAsync(Guid searchId);

        /// <exception cref="Models.Searchs.Exceptions.SearchValidationException"></exception>
        /// <exception cref="Models.Searchs.Exceptions.SearchDependencyValidationException"></exception>
        /// <exception cref="Models.Searchs.Exceptions.SearchDependencyException"></exception>
        /// <exception cref="Models.Searchs.Exceptions.SearchServiceException"></exception>
        ValueTask<Search> ModifySearchAsync(Search search);

        /// <exception cref="Models.Searchs.Exceptions.SearchDependencyValidationException"></exception>
        /// <exception cref="Models.Searchs.Exceptions.SearchDependencyException"></exception>
        /// <exception cref="Models.Searchs.Exceptions.SearchServiceException"></exception>
        ValueTask<Search> RemoveSearchByIdAsync(Guid searchId);
    }
}