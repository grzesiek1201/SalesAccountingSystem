namespace AccountingSystem.Domain.Enums
{

    public enum CustomerAddResult
    {
        Success,
        InvalidData
    }
    public enum CustomerEditResult
    {
        Success,
        NotFound,
        InvalidData,
        CustomerArchived
    }

    public enum CustomerArchiveResult
    {
        Success,
        NotFound,
        AlreadyArchived,
        CustomerInDebt
    } 

    public enum ValidateCustomerResult
    {
       IsValid,
       NotValid
        
    }

}
