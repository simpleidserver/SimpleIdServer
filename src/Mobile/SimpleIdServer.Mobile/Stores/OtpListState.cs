using SimpleIdServer.Mobile.Models;
using System.Collections.ObjectModel;

namespace SimpleIdServer.Mobile.Stores
{
    public class OtpListState
    {
        private readonly MobileDatabase _database;

        public OtpListState()
        {
            _database = App.Database;
        }

        public ObservableCollection<OTPCode> OTPCodes { get; set; } = new ObservableCollection<OTPCode>();

        public async Task Load()
        {
            var otpCodes = await _database.GetOTPCodes();
            foreach (var otpCode in otpCodes) OTPCodes.Add(otpCode);
        }

        public async Task UpdateOTPCode(OTPCode otpCode)
        {
            await _database.UpdateOTPCode(otpCode);
            OTPCodes.Remove(otpCode);
            OTPCodes.Add(otpCode);
        }

        public async Task AddOTPCode(OTPCode otpCode)
        {
            await _database.AddOTPCode(otpCode);
            OTPCodes.Add(otpCode);
        }

        public async Task RemoveOTPCode(OTPCode otpCode)
        {
            await _database.RemoveOTPCode(otpCode);
            OTPCodes.Remove(otpCode);
        }
    }
}
