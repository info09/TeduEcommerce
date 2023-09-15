using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeduEcommerce.IdentitySettings;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace TeduEcommerce.Products
{
    public class ProductAttributeCodeGenerator : ITransientDependency
    {
        private readonly IRepository<IdentitySetting, string> _identitySettingRepository;

        public ProductAttributeCodeGenerator(IRepository<IdentitySetting, string> identitySettingRepository)
        {
            this._identitySettingRepository = identitySettingRepository;
        }

        public async Task<string> GenerateAsync()
        {
            string newCode;
            var identitySetting = await _identitySettingRepository.FindAsync(TeduEcommerceConsts.ProductAttributeIdentitySettingId);
            if (identitySetting == null)
            {
                identitySetting = await _identitySettingRepository.InsertAsync(
                                                                                new IdentitySetting(TeduEcommerceConsts.ProductAttributeIdentitySettingId,
                                                                                "Thuộc tính sản phẩm",
                                                                                TeduEcommerceConsts.ProductAttributeIdentitySettingPrefix,
                                                                                1,
                                                                                1));
                newCode = identitySetting.Prefix + identitySetting.CurrentNumber;
            }
            else
            {
                identitySetting.CurrentNumber += identitySetting.StepNumber;
                newCode = identitySetting.Prefix + identitySetting.CurrentNumber;

                await _identitySettingRepository.UpdateAsync(identitySetting);
            }

            return newCode;
        }
    }
}
