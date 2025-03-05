using System;
using System.Collections.Generic;

namespace Bank_Management_System.models;

public partial class Loan
{
    public Guid LoanId { get; set; }

    public Guid? UserId { get; set; }

    public string? LoanType { get; set; }

    public decimal LoanAmount { get; set; }

    public decimal InterestRate { get; set; }

    public int LoanTerm { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
