namespace TeduEcommerce.Admin.System.Users{
    public class UserInListDto:AuditedEntityDto{
        public string Name {get;set;}
        public string SurName {get;set;}
        public string Email {get;set;}
        public string UserName {get;set;}
        public string PhoneNumber {get;set;}
    }
}