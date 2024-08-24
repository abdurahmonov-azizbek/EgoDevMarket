// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by Ego Devs
//  --------------------------------------------------------

namespace EgoDevMarket.Api.Models.Users
{
	public class User{
	    public Guid Id { get; set; }
	    public string FirstName { get; set;}
	    public string LastName { get; set;}
	    public string PhoneNumber{get;set;}
	    public string Role{get;set;}
	    public string Password{get;set;}
	    public DateTimeOffset CreatedDate { get; set; }
	    public DateTimeOffset UpdatedDate { get; set; }
	}
}