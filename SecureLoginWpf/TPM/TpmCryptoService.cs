using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SecureLoginWpf.TPM
{
    public static class TpmCryptoService
    {
        public const string KEY_NAME = "SecureLoginWpfKey";
        public const string PUBKEY_PATH = "registeredPublicKey.der";

        public static CngKey GenerateTPMBoundKey()
        {
            if (CngKey.Exists(KEY_NAME, CngProvider.MicrosoftPlatformCryptoProvider))
            {
                using var existing = CngKey.Open(KEY_NAME, CngProvider.MicrosoftPlatformCryptoProvider);
                existing.Delete();
            }

            var cp = new CngKeyCreationParameters
            {
                Provider = CngProvider.MicrosoftPlatformCryptoProvider,
                KeyUsage = CngKeyUsages.Signing,
                KeyCreationOptions = CngKeyCreationOptions.OverwriteExistingKey
            };

            using var key = CngKey.Create(CngAlgorithm.ECDsa, KEY_NAME, cp);

            using var ecdsa = new ECDsaCng(key);
            byte[] pubDer = ecdsa.ExportSubjectPublicKeyInfo();
            File.WriteAllBytes(PUBKEY_PATH, pubDer);

            return key;
        }

        public static byte[] LoadRegisteredPublicKey()
        {
            if (!File.Exists(PUBKEY_PATH))
                throw new InvalidOperationException("Public key not found. Have you registered yet?");
            return File.ReadAllBytes(PUBKEY_PATH);
        }

    }
}
