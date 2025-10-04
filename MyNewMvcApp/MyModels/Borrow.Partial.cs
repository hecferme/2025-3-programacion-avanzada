using System;

namespace ProgramacionAvanzada.Books.Model
{
    public partial class Borrow
    {
    public int BorrowDays => (FinalDate.ToDateTime(TimeOnly.MinValue) - InitialDate.ToDateTime(TimeOnly.MinValue)).Days;
    public int? RealBorrowDays => RealDevolutionDate.HasValue ? (RealDevolutionDate.Value.ToDateTime(TimeOnly.MinValue) - InitialDate.ToDateTime(TimeOnly.MinValue)).Days : (int?)null;
    public int? DelayDays => RealDevolutionDate.HasValue ? (RealDevolutionDate.Value.ToDateTime(TimeOnly.MinValue) - FinalDate.ToDateTime(TimeOnly.MinValue)).Days : (int?)null;
    }
}
