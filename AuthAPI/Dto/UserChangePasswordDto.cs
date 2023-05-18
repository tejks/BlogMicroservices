namespace AuthAPI.Dto
{
    public class UserChangePasswordDto
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
