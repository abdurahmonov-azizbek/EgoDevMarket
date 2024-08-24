// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

namespace EgoDevMarket.Api.Models.Searchs
{
	public class Search{
	    public Guid Id{get;set;}
	    public string Keyword{get;set;}
	    public int Quantity{get;set;}
	    public DateTimeOffset CreatedDate { get; set; }
	    public DateTimeOffset UpdatedDate { get; set; }
	}
}