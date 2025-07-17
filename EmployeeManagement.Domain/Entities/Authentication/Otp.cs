namespace EmployeeManagement.Domain.Entities.Authentication
{
    public class Otp
    {
        public int Id { get; set; }
        public int UserOtp { get; set; }
        public string UserId { get; set; }
        public DateTime OtpExpTime { get; set; }


    }
}
