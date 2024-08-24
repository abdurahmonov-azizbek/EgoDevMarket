// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

namespace EgoDevMarket.Api.Models.Products
{
	public class Product{
	    public Guid Id { get; set;}
	    public string Name { get; set;}
	    public float Price { get; set;}
	    public string Description{get;set;}
	    public int SellerId{ get; set;}
	    public int CategoryId{get;set;}
	    public DateTimeOffset CreatedDate { get; set; }
	    public DateTimeOffset UpdatedDate { get; set; }
	}
}