using AutoMapper;
using CreditWiseHub.Core.Dtos.Account;
using CreditWiseHub.Core.Dtos.Transactions;
using CreditWiseHub.Core.Models;

namespace CreditWiseHub.Service.Mapping
{
    public class AccountMap : Profile
    {
        public AccountMap()
        {
            CreateMap<CreateAccountDto, Account>()
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.OpeningBalance))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<Account, AccountInfoDto>()
                .ForMember(dest => dest.AccountTypeName, opt => opt.MapFrom(src => src.AccountType.Name));
            CreateMap<Account, AccountLastInfoDto>()
                .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Name));

            CreateMap<Account, AccountHistoryDto>()
           .ForMember(dest => dest.AccountNumber, opt => opt.MapFrom(src => src.AccountNumber))
           .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Name))
           .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
           .ForMember(dest => dest.AccountHistories, opt => opt.MapFrom(src => src.TransactionAffectedAccounts.Select(x => new TransactionDetailForAccountHistoryDto
           {
               TransactionAmount = x.Transaction.Amount,
               AfterTransactionBalance = (decimal)x.AfterBalance!,
               BeforeTransactionBalance = (decimal)x.BeforeBalance!,
               Description = x.Description,
               TransactionDateTime = DateTime.SpecifyKind(x.Transaction.TransactionDate, DateTimeKind.Local),
           }).ToList()));

            CreateMap<TransactionAffectedAccount, TransactionDetailForAccountHistoryDto>();


            CreateMap<AffectedAccountDto, TransactionAffectedAccount>();
            CreateMap<AffectedAccountDto, ExternalAccountInformation>();
            CreateMap<AffectedExternalAccountDto, ExternalAccountInformation>();
            CreateMap<Account, RecipientAccountInfoDto>()
                .ForMember(dest => dest.OwnerFullName, opt => opt.MapFrom(src => GenerateUserNameWithCensor($"{src.UserApp.Name} {src.UserApp.Surname}")))
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.Id));
                


        }

        public string GenerateUserNameWithCensor(string userName)
        {
            string result = "";

            string[] words = userName.Split(' ');

            foreach (var word in words)
            {
                if (word.Length > 1)
                {
                    result += $"{word.Substring(0, 1)}{new string('*', word.Length - 1)} ";
                }
                else
                {
                    result += $"{word} ";
                }
            }

            return result.Trim();
        }
    }
}
