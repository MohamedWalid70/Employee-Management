namespace Internship.EmployeeManagement.Core.Interfaces
{
    public interface IRequestValidation<TShared>
    {
        public Guid EntityId { get; set; }
        public TShared? SharedObject { get; set; }
    }
}
