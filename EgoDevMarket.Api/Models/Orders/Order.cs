// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

namespace EgoDevMarket.Api.Models.Orders
{
	public class Order{
	    public Guid Id { get; set;}
	    public int UserId { get; set;}
	    public int ProductId { get; set;}
	    public DateTimeOffset CreatedDate { get; set; }
	    public DateTimeOffset UpdatedDate { get; set; }
	}
}