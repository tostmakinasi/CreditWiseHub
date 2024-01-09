using CreditWiseHub.Core.Dtos;
using CreditWiseHub.Core.Dtos.AccountType;
using CreditWiseHub.Core.Responses;

namespace CreditWiseHub.Core.Abstractions.Services
{
    /// <summary>
    /// Interface for managing account types through service operations.
    /// </summary>
    public interface IAccountTypeService
    {
        /// <summary>
        /// Retrieves all account types.
        /// </summary>
        /// <returns>A response containing a list of account types, or an error message if unsuccessful.</returns>
        Task<Response<List<AccountTypeDto>>> GetAll();

        /// <summary>
        /// Retrieves an account type by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the account type to retrieve.</param>
        /// <returns>A response containing the account type details, or an error message if unsuccessful.</returns>
        Task<Response<AccountTypeDetailDto>> GetById(int id);

        /// <summary>
        /// Creates a new account type based on the provided data.
        /// </summary>
        /// <param name="accountTypeDto">The data to create the new account type.</param>
        /// <returns>A response containing the created account type details, or an error message if unsuccessful.</returns>
        Task<Response<AccountTypeDetailDto>> CreateAsync(CreateAccountTypeDto accountTypeDto);

        /// <summary>
        /// Updates an existing account type based on the provided data.
        /// </summary>
        /// <param name="id">The identifier of the account type to update.</param>
        /// <param name="accountTypeDto">The data to update the account type.</param>
        /// <returns>A response indicating success or failure, along with an error message if applicable.</returns>
        Task<Response<NoDataDto>> Update(int id, UpdateAccountTypeDto accountTypeDto);

        /// <summary>
        /// Deletes an account type by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the account type to delete.</param>
        /// <returns>A response indicating success or failure, along with an error message if applicable.</returns>
        Task<Response<NoDataDto>> Delete(int id);
    }

}
