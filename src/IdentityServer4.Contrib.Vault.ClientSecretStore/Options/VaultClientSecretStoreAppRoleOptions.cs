﻿// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Vault.ClientSecretStore.Options
{
    public class VaultClientSecretStoreAppRoleOptions : VaultClientSecretStoreOptions
    {
        public string RoleId { get; set; }

        public string SecretId { get; set; }
    }
}
