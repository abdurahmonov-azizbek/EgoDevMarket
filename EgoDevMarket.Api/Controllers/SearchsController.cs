// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EgoDevMarket.Api.Models.Searchs;
using EgoDevMarket.Api.Models.Searchs.Exceptions;
using EgoDevMarket.Api.Services.Foundations.Searchs;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace EgoDevMarket.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchsController : RESTFulController
    {
        private readonly ISearchService searchService;

        public SearchsController(ISearchService searchService) =>
            this.searchService = searchService;

        [HttpPost]
        public async ValueTask<ActionResult<Search>> PostSearchAsync(Search search)
        {
            try
            {
                Search addedSearch = await this.searchService.AddSearchAsync(search);

                return Created(addedSearch);
            }
            catch (SearchValidationException searchValidationException)
            {
                return BadRequest(searchValidationException.InnerException);
            }
            catch (SearchDependencyValidationException searchDependencyValidationException)
                when (searchDependencyValidationException.InnerException is AlreadyExistsSearchException)
            {
                return Conflict(searchDependencyValidationException.InnerException);
            }
            catch (SearchDependencyException searchDependencyException)
            {
                return InternalServerError(searchDependencyException.InnerException);
            }
            catch (SearchServiceException searchServiceException)
            {
                return InternalServerError(searchServiceException.InnerException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<Search>> GetAllSearchs()
        {
            try
            {
                IQueryable<Search> allSearchs = this.searchService.RetrieveAllSearchs();

                return Ok(allSearchs);
            }
            catch (SearchDependencyException searchDependencyException)
            {
                return InternalServerError(searchDependencyException.InnerException);
            }
            catch (SearchServiceException searchServiceException)
            {
                return InternalServerError(searchServiceException.InnerException);
            }
        }

        [HttpGet("{searchId}")]
        public async ValueTask<ActionResult<Search>> GetSearchByIdAsync(Guid searchId)
        {
            try
            {
                return await this.searchService.RetrieveSearchByIdAsync(searchId);
            }
            catch (SearchDependencyException searchDependencyException)
            {
                return InternalServerError(searchDependencyException);
            }
            catch (SearchValidationException searchValidationException)
                when (searchValidationException.InnerException is InvalidSearchException)
            {
                return BadRequest(searchValidationException.InnerException);
            }
            catch (SearchValidationException searchValidationException)
                 when (searchValidationException.InnerException is NotFoundSearchException)
            {
                return NotFound(searchValidationException.InnerException);
            }
            catch (SearchServiceException searchServiceException)
            {
                return InternalServerError(searchServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Search>> PutSearchAsync(Search search)
        {
            try
            {
                Search modifiedSearch =
                    await this.searchService.ModifySearchAsync(search);

                return Ok(modifiedSearch);
            }
            catch (SearchValidationException searchValidationException)
                when (searchValidationException.InnerException is NotFoundSearchException)
            {
                return NotFound(searchValidationException.InnerException);
            }
            catch (SearchValidationException searchValidationException)
            {
                return BadRequest(searchValidationException.InnerException);
            }
            catch (SearchDependencyValidationException searchDependencyValidationException)
            {
                return BadRequest(searchDependencyValidationException.InnerException);
            }
            catch (SearchDependencyException searchDependencyException)
            {
                return InternalServerError(searchDependencyException.InnerException);
            }
            catch (SearchServiceException searchServiceException)
            {
                return InternalServerError(searchServiceException.InnerException);
            }
        }

        [HttpDelete("{searchId}")]
        public async ValueTask<ActionResult<Search>> DeleteSearchByIdAsync(Guid searchId)
        {
            try
            {
                Search deletedSearch = await this.searchService.RemoveSearchByIdAsync(searchId);

                return Ok(deletedSearch);
            }
            catch (SearchValidationException searchValidationException)
                when (searchValidationException.InnerException is NotFoundSearchException)
            {
                return NotFound(searchValidationException.InnerException);
            }
            catch (SearchValidationException searchValidationException)
            {
                return BadRequest(searchValidationException.InnerException);
            }
            catch (SearchDependencyValidationException searchDependencyValidationException)
                when (searchDependencyValidationException.InnerException is LockedSearchException)
            {
                return Locked(searchDependencyValidationException.InnerException);
            }
            catch (SearchDependencyValidationException searchDependencyValidationException)
            {
                return BadRequest(searchDependencyValidationException.InnerException);
            }
            catch (SearchDependencyException searchDependencyException)
            {
                return InternalServerError(searchDependencyException.InnerException);
            }
            catch (SearchServiceException searchServiceException)
            {
                return InternalServerError(searchServiceException.InnerException);
            }
        }
    }
}