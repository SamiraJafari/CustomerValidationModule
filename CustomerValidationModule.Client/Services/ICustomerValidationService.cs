using CustomerValidationModule.Client.Models;

namespace CustomerValidationModule.Client.Services;

public interface ICustomerValidationService
{
    Task<CustomerValidationResult> GetCustomerValidationAsync(string nationalCode, CancellationToken cancellationToken = default);
}
