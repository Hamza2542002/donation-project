namespace donation_project.models
{
    public class ForgetPasswordModel
    {
        public string Message { get; set; }
        //public string OTP { get; set; }
        public DateTime OTPExpiration { get; set; }
        public bool SentSuccessfully { get; set; }
    }
}
