using System;
using System.Collections.Generic;

namespace Bank_Management_System.models;

public partial class Account
{
    public Guid AccountId { get; set; }

    public Guid? UserId { get; set; }

    public string AccountNumber { get; set; } = null!;

    public string? AccountType { get; set; }

    public decimal? Balance { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public virtual User? User { get; set; }
}
