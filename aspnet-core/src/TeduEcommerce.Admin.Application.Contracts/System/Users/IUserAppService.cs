namespace TeduEcommerce.Admin.System.Users{
    public interface IUserAppService : ICrudAppService<UserDto, Guid, PagedResultRequestDto, CreateUserDto, UpdateUserDto>{
        Task DeleteMultipleAsync(IEnumerable<Guid> ids);
        Task<PagedResultDto<UserInListDto>> GetListWithFilterAsync(BaseListFilterDto input);
        Task<List<UserInListDto>> GetListAllAsync(string filterKeyword);
    }
}