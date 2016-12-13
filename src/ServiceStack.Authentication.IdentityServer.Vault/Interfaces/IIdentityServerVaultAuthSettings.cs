﻿// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Authentication.IdentityServer.Vault.Interfaces
{
    public interface IIdentityServerVaultAuthSettings
    {
        string VaultUri { get; set; }

        string VaultAppId { get; set; }

        string VaultUserId { get; set; }

        string VaultEncryptionKey { get; set; }
    }
}
