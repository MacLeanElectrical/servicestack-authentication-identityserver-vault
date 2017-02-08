﻿namespace ServiceStack.Vault.Core.VaultAuth
{
    using System;
    using DTO;
    using Helpers;
    using Interfaces;
    using Logging;

    public class VaultAppRoleAuth : IVaultAuth
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(VaultAppRoleAuth));

        private readonly string roleId;
        private readonly string secretId;

        public VaultAppRoleAuth(string roleId, string secretId)
        {
            this.roleId = roleId.ThrowIfNullOrWhitespace(nameof(roleId));
            this.secretId = secretId.ThrowIfNullOrWhitespace(nameof(secretId));
        }

        public void Authenticate(IVaultClientUri vaultUri)
        {
            try
            {
                using (var client = vaultUri.ServiceClient)
                {
                    var result = client.Post(new AppRoleLogin
                    {
                        RoleId = roleId,
                        SecretId = secretId
                    });

                    AuthToken = result.Auth.ClientToken;
                }
            }
            catch (Exception exception)
            {
                Log.Error("Unable to authenticate client using AppRole", exception);
                throw;
            }
        }

        public string AuthToken { get; set; }
    }
}