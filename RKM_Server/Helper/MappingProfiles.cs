using AutoMapper;
using RKM_Server.DTO;
using RKM_Server.Models;

namespace RKM_Server.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<StockItem, StockItemDto>();
            CreateMap<StockItemDto, StockItem>();

            CreateMap<Project, ProjectDto>();
            CreateMap<ProjectDto, Project>();

            CreateMap<ProjectAccount, ProjectAccountDto>();
            CreateMap<ProjectAccountDto, ProjectAccount>();

            CreateMap<Stock, StockDto>();
            CreateMap<StockDto, Stock>();

            CreateMap<StockAccount, StockAccountDto>();
            CreateMap<StockAccountDto, StockAccount>();

            CreateMap<Orderer, OrdererDto>();
            CreateMap<OrdererDto, Orderer>();

        }
    }
}
