using System.Threading.Tasks;
using Windows.Security.Credentials.UI;

namespace SecureLoginWPF.Services
{
    public static class WindowsHelloService
    {
        public static async Task<bool> RequestBiometricVerificationAsync()
        {
            var consentResult = await UserConsentVerifier.RequestVerificationAsync("Please verify your identity with Windows Hello");
            return consentResult == UserConsentVerificationResult.Verified;
        }

        public static async Task<bool> CheckAvailabilityAsync()
        {
            var availability = await UserConsentVerifier.CheckAvailabilityAsync();
            return availability == UserConsentVerifierAvailability.Available;
        }
    }
}
