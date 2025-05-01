using SecureLoginWpf.Helpers;
using SecureLoginWpf.Models;
using SecureLoginWpf.TPM;
using SecureLoginWPF.Services;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SecureLoginWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //private async void Login_Click(object sender, RoutedEventArgs e)
        //{
        //    // Windows Hello Check
        //    if (!await WindowsHelloService.CheckAvailabilityAsync())
        //    {
        //        MessageBox.Show("Windows Hello is not available or not set up.", "Biometric Auth", MessageBoxButton.OK, MessageBoxImage.Warning);
        //        return;
        //    }

        //    bool verified = await WindowsHelloService.RequestBiometricVerificationAsync();
        //    if (!verified)
        //    {
        //        MessageBox.Show("Biometric authentication failed or was canceled.", "Login", MessageBoxButton.OK, MessageBoxImage.Warning);
        //        return;
        //    }            

        //    // TPM Key Flow
        //    if (!TpmCryptoService.GenerateTPMBoundKey())
        //    {
        //        MessageBox.Show("TPM key generation failed.", "TPM Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //        return;
        //    }

        //    MessageBox.Show("Login successful using Windows Hello and TPM-backed key!", "Login", MessageBoxButton.OK, MessageBoxImage.Information);

        //    //var encryptedPassword = EncryptHelper.Encrypt(password);
        //    //MessageBox.Show($"Login successful with Windows Hello.\nUsername: {username}\nEncrypted Password: {encryptedPassword}", "Login", MessageBoxButton.OK, MessageBoxImage.Information);
        //}
        private async void OnRegister(object sender, RoutedEventArgs e)
        {
            // Windows Hello Check
            if (!await WindowsHelloService.CheckAvailabilityAsync())
            {
                txtOutput.Text = "Windows Hello is not available or not set up.";
                return;
            }

            bool verified = await WindowsHelloService.RequestBiometricVerificationAsync();
            if (!verified)
            {
                txtOutput.Text = "Biometric authentication failed or was canceled.";
            }

            var key = TpmCryptoService.GenerateTPMBoundKey();
            txtOutput.Text = "Registered in TPM";
        }

        private async void OnAuthenticate(object sender, RoutedEventArgs e)
        {
            // Windows Hello Check
            if (!await WindowsHelloService.CheckAvailabilityAsync())
            {
                txtOutput.Text = "Windows Hello is not available or not set up.";
                return;
            }

            bool verified = await WindowsHelloService.RequestBiometricVerificationAsync();
            if (!verified)
            {
                txtOutput.Text = "Biometric authentication failed or was canceled.";
            }

            if (!CngKey.Exists(
            TpmCryptoService.KEY_NAME,
            CngProvider.MicrosoftPlatformCryptoProvider
            ))
            {
                txtOutput.Text = "No credential";
                return;
            }
            using var key = CngKey.Open(
                    TpmCryptoService.KEY_NAME,
                    CngProvider.MicrosoftPlatformCryptoProvider
                );
            using var alg = new ECDsaCng(key);

            byte[] challenge = Encoding.UTF8.GetBytes("server_challenge");
            byte[] sig = alg.SignData(challenge);

            txtOutput.Text = Convert.ToBase64String(sig);

            bool valid;
            byte[] publicKeyDer = TpmCryptoService.LoadRegisteredPublicKey();
            using (var ecdsa = new ECDsaCng())
            {
                ecdsa.ImportSubjectPublicKeyInfo(publicKeyDer, out _);
                valid = ecdsa.VerifyData(challenge, sig, HashAlgorithmName.SHA256);
            }

            txtOutput.Text += valid
            ? "\n Authentication successful."
            : "\n Signature invalid!";
        }
    }
}