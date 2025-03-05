using System;
using System.Collections.Generic;

namespace Bank_Management_System.models;

public partial class Transaction
{
    public Guid TransactionId { get; set; }

    public Guid? AccountId { get; set; }

    public string Type { get; set; } = null!;

    public decimal Amount { get; set; }

    public string? RecipientAccountNumber { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Account? Account { get; set; }
}
