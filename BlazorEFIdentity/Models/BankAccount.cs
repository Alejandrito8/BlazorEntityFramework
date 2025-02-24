using System;
using Microsoft.Identity.Client;

namespace BlazorEFIdentity.Models
{
public class BankAccount
{
public int Id { get; set; }
public string AccountNumber { get; set; }
public string Name { get; set; }
public decimal Balance { get; set; }
public string Type { get; set; }
public int? CardNumber {get; set;}
public bool Active {get; set;}
public string UserId {get; set;}
public List<Transaction> Transaction {get; set;} = new List <Transaction>();
}
}