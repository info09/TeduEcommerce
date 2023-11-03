﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace TeduEcommerce.Admin.System.Users
{
    [Authorize(IdentityPermissions.Users.Default, Policy = "AdminOnly")]
    public class UserAppService : CrudAppService<IdentityUser, UserDto, Guid, PagedResultRequestDto, CreateUserDto, UpdateUserDto>, IUserAppService
    {
        private readonly IdentityUserManager _identityUserManager;
        public UserAppService(IRepository<IdentityUser, Guid> repository, IdentityUserManager identityUserManager) : base(repository)
        {
            _identityUserManager = identityUserManager;

            GetPolicyName = IdentityPermissions.Users.Default;
            GetListPolicyName = IdentityPermissions.Users.Default;
            CreatePolicyName = IdentityPermissions.Users.Create;
            UpdatePolicyName = IdentityPermissions.Users.Update;
            DeletePolicyName = IdentityPermissions.Users.Delete;
        }

        [Authorize(IdentityPermissions.Users.Create)]
        public async override Task<UserDto> CreateAsync(CreateUserDto input)
        {
            var query = await Repository.GetQueryableAsync();
            var isUserNameExisted = query.Any(i => i.UserName.ToLower() == input.UserName.ToLower());
            if (isUserNameExisted)
                throw new UserFriendlyException("Tài khoản đã tồn tại");

            var isEmailExisted = query.Any(i => i.Email.ToLower() == input.Email.ToLower());
            if (isEmailExisted)
                throw new UserFriendlyException("Email đã tồn tại");

            var userId = Guid.NewGuid();
            var user = new IdentityUser(userId, input.UserName, input.Email);
            user.Name = input.Name;
            user.Surname = input.Surname;
            user.SetPhoneNumber(input.PhoneNumber, true);
            var result = await _identityUserManager.CreateAsync(user, input.Password);
            if (result.Succeeded)
                return ObjectMapper.Map<IdentityUser, UserDto>(user);
            else
            {
                List<IdentityError> errorList = result.Errors.ToList();
                string err = "";

                foreach (var item in errorList)
                {
                    err = err + item.Description.ToString();
                }
                throw new UserFriendlyException(err);
            }
        }

        [Authorize(IdentityPermissions.Users.Update)]
        public override async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto input)
        {
            var user = await _identityUserManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new EntityNotFoundException(typeof(IdentityUser), id);
            }

            user.Name = input.Name;
            user.SetPhoneNumber(input.PhoneNumber, true);
            user.Surname = input.Surname;
            var result = await _identityUserManager.UpdateAsync(user);
            if (result.Succeeded)
                return ObjectMapper.Map<IdentityUser, UserDto>(user);
            else
            {
                List<IdentityError> errorList = result.Errors.ToList();
                string err = "";

                foreach (var item in errorList)
                {
                    err = err + item.Description.ToString();
                }
                throw new UserFriendlyException(err);
            }
        }

        [Authorize(IdentityPermissions.Users.Delete)]
        public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
        {
            await Repository.DeleteManyAsync(ids);
            await UnitOfWorkManager.Current.SaveChangesAsync();
        }

        [Authorize(IdentityPermissions.Users.Default)]
        public async Task<List<UserInListDto>> GetListAllAsync(string filterKeyword)
        {
            var query = await Repository.GetQueryableAsync();
            query = query.WhereIf(!string.IsNullOrEmpty(filterKeyword), i => i.Name.ToLower().Contains(filterKeyword.ToLower()) ||
                                                                                i.Email.ToLower().Contains(filterKeyword.ToLower()) ||
                                                                                i.PhoneNumber.ToLower().Contains(filterKeyword.ToLower()));

            var data = await AsyncExecuter.ToListAsync(query);
            return ObjectMapper.Map<List<IdentityUser>, List<UserInListDto>>(data);
        }

        [Authorize(IdentityPermissions.Users.Default)]
        public async Task<PagedResultDto<UserInListDto>> GetListWithFilterAsync(BaseListFilterDto input)
        {
            var query = await Repository.GetQueryableAsync();

            query = query.WhereIf(!string.IsNullOrWhiteSpace(input.Keyword), i => i.Name.ToLower().Contains(input.Keyword.ToLower()) ||
                                                                                    i.Email.ToLower().Contains(input.Keyword.ToLower()) ||
                                                                                    i.PhoneNumber.ToLower().Contains(input.Keyword.ToLower()));

            query = query.OrderByDescending(x => x.CreationTime);
            var totalCount = await AsyncExecuter.LongCountAsync(query);

            query = query.Skip(input.SkipCount).Take(input.MaxResultCount);
            var data = await AsyncExecuter.ToListAsync(query);

            var users = ObjectMapper.Map<List<IdentityUser>, List<UserInListDto>>(data);

            return new PagedResultDto<UserInListDto>(totalCount, users);
        }

        [Authorize(IdentityPermissions.Users.Default)]
        public override async Task<UserDto> GetAsync(Guid id)
        {
            var user = await _identityUserManager.GetByIdAsync(id);
            if (user == null)
            {
                throw new EntityNotFoundException(typeof(IdentityUser), id);
            }

            var userDto = ObjectMapper.Map<IdentityUser, UserDto>(user);
            var roles = await _identityUserManager.GetRolesAsync(user); 
            userDto.Roles = roles;

            return userDto;
        }

        [Authorize(IdentityPermissions.Users.Update)]
        public async Task AssignRolesAsync(Guid userId, string[] roleNames)
        {
            var user = await _identityUserManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new EntityNotFoundException(typeof(IdentityUser), userId);
            }

            var currentRoles = await _identityUserManager.GetRolesAsync(user);
            var removedResult = await _identityUserManager.RemoveFromRolesAsync(user, currentRoles);
            var addedResult = await _identityUserManager.AddToRolesAsync(user, roleNames);
            if (!addedResult.Succeeded && !removedResult.Succeeded)
            {
                List<IdentityError> addedErrorList = addedResult.Errors.ToList();
                List<IdentityError> removedErrorList = removedResult.Errors.ToList();
                var errorList = new List<IdentityError>();
                errorList.AddRange(addedErrorList);
                errorList.AddRange(removedErrorList);
                string errors = "";
                foreach (var err in errorList)
                {
                    errors += err.Description.ToString();
                }
                throw new UserFriendlyException(errors);
            }
        }

        [Authorize(IdentityPermissions.Users.Update)]
        public async Task SetPasswordAsync(Guid userId, SetPasswordDto input)
        {
            var user = await _identityUserManager.FindByIdAsync(userId.ToString()) ?? throw new EntityNotFoundException(typeof(IdentityUser), userId);
            var token = await _identityUserManager.GeneratePasswordResetTokenAsync(user);
            var result = await _identityUserManager.ResetPasswordAsync(user, token, input.NewPassword);

            if (!result.Succeeded)
            {
                List<IdentityError> errorList = result.Errors.ToList();
                string errors = "";

                foreach (var err in errorList)
                {
                    errors += err.Description.ToString();
                }

                throw new UserFriendlyException(errors);
            }
        }
    }
}
