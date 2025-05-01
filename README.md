## WebAuthn Windows Client Demo

This repository contains a proof-of-concept WPF application demonstrating a native Windows WebAuthn authentication client. It showcases:

- **TPM-bound key generation** using CNG (MicrosoftPlatformCryptoProvider)
- **ECDSA-P256** signing of server challenges
- **Windows Hello** user verification via UserConsentVerifier
- **Signature verification** using the stored public key

---

### Table of Contents

- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Usage](#usage)
- [Demo Flow](#demo-flow)
- [Extending the Demo](#extending-the-demo)
- [Contributing](#contributing)
- [License](#license)

---

## Prerequisites

- Windows 10 or 11 with TPM support and Windows Hello enabled
- Visual Studio 2022 or later with `.NET 8` workload:
  - **.NET desktop development**
  - **Windows 10/11 SDK**
- Git (for cloning the repo)

---

## Getting Started

Clone the repository:

```bash
git clone https://github.com/yourusername/WebAuthnWinClientDemo.git
cd WebAuthnWinClientDemo
```

Open the solution file `WebAuthnWinClientDemo.sln` in Visual Studio. Restore NuGet packages and build the solution.

---

## Project Structure

```text
WebAuthnWinClientDemo/
├── src/
│   ├── WindowsHelloService.cs     # Windows Hello availability & prompt
│   ├── TpmCryptoService.cs        # TPM-bound key generation & public key persistence
│   └── MainWindow.xaml(.cs)       # WPF UI and authentication flow
├── registeredPublicKey.der        # Persisted public key (auto-generated at registration)
└── WebAuthnWinClientDemo.sln      # Visual Studio solution
```

---

## Usage

1. **Register**
   - Launch the app and click **Register**.
   - Windows Hello prompt appears; verify your identity.
   - A TPM-bound ECDSA key is generated and the public key is saved to `registeredPublicKey.der`.

2. **Authenticate**
   - Click **Authenticate**.
   - Windows Hello prompt appears again.
   - The app signs a fixed challenge (`"server_challenge"`) with the TPM key.
   - The signature is verified against the stored public key.
   - A success or failure message is displayed.

---

## Demo Flow

1. **Check Windows Hello** availability via `UserConsentVerifier.CheckAvailabilityAsync()`.
2. **Prompt for verification** via `UserConsentVerifier.RequestVerificationAsync()`.
3. **Generate a TPM-bound key** with `TpmCryptoService.GenerateTPMBoundKey()`.
4. **Sign** the challenge using `ECDsaCng.SignData()`.
5. **Export** and **persist** the public key in DER format.
6. **Verify** the signature in `OnAuthenticate` by importing the persisted public key and calling `ECDsaCng.VerifyData()`.

---

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

---

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
