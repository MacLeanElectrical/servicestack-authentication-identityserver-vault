﻿namespace IdSvr4.Vault.CertStore.Demo
{
    using System;
    using System.Net.Http;
    using ServiceStack;
    using ServiceStack.Text;

    public static class VaultUriExtensions
    {
        public static void Initialize(this string vaultUri, out string rootToken, out string[] keys)
        {
            using (var client = new JsonServiceClient(vaultUri))
            {
                var response = client.Put<JsonObject>("v1/sys/init", new { secret_shares = 5, secret_threshold = 3 });

                rootToken = response["root_token"];
                keys = response["keys"].FromJson<string[]>();
            }
        }

        public static void Unseal(this string vaultUri, string[] keys)
        {
            using (var client = new JsonServiceClient(vaultUri))
            {
                for (var i = 0; i < keys.Length; i++)
                {
                    var response = client.Put<JsonObject>("v1/sys/unseal", new { key = keys[i] });
                    if (response["sealed"] == "false")
                    {
                        break;
                    }
                }
            }
        }

        public static void MountTransit(this string vaultUri, string rootToken)
        {
            using (var client = new JsonServiceClient(vaultUri))
            {
                client.AddHeader("X-Vault-Token", rootToken);
                client.Post<JsonObject>("v1/sys/mounts/transit", new { type = "transit" });
            }
        }

        public static void MountPki(this string vaultUri, string rootToken)
        {
            using (var client = new JsonServiceClient(vaultUri))
            {
                client.AddHeader("X-Vault-Token", rootToken);
                client.Post<JsonObject>("v1/sys/mounts/pki", new { type = "pki" });
            }
        }

        public static void MountTunePki(this string vaultUri, string rootToken)
        {
            using (var client = new JsonServiceClient(vaultUri))
            {
                client.AddHeader("X-Vault-Token", rootToken);
                client.Post<JsonObject>("v1/sys/mounts/pki/tune", new { max_lease_ttl = "87600h" });
            }
        }

        public static void GenerateRootCertificate(this string vaultUri, string rootToken, string cn, string ttl)
        {
            using (var client = new JsonServiceClient(vaultUri))
            {
                client.AddHeader("X-Vault-Token", rootToken);
                var result = client.Post<JsonObject>("v1/pki/root/generate/internal", new { common_name = cn, ttl });
            }
        }

        public static void SetCertificateUrlConfiguration(this string vaultUri, string rootToken)
        {
            using (var client = new JsonServiceClient(vaultUri))
            {
                client.AddHeader("X-Vault-Token", rootToken);
                client.Post<JsonObject>("v1/pki/config/urls",
                    new
                    {
                        issuing_certificates = $"{vaultUri}/v1/pki/ca",
                        crl_distribution_points = $"{vaultUri}/v1/pki/crl"
                    });
            }
        }

        public static void GetCertificateUrlConfiguration(this string vaultUri, string rootToken)
        {
            using (var client = new JsonServiceClient(vaultUri))
            {
                client.AddHeader("X-Vault-Token", rootToken);
                var response = client.Get<JsonObject>("v1/pki/config/urls");
            }
        }

        public static void GenerateCertificateRole(this string vaultUri, string rootToken, string roleName, string domains)
        {
            using (var client = new JsonServiceClient(vaultUri))
            {
                client.AddHeader("X-Vault-Token", rootToken);
                client.Post<JsonObject>($"v1/pki/roles/{roleName}", new
                {
                    allowed_domains = domains,
                    allow_subdomains = true,
                    max_ttl = "72h"
                });
            }
        }

        public static void CreateEncryptionKey(this string vaultUri, string rootToken, string encryptionKey)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(vaultUri) })
            {
                client.DefaultRequestHeaders.Add("X-Vault-Token", rootToken);
                var response = client.PostAsync($"/v1/transit/keys/{encryptionKey}", null).Result;
            }
        }

        [Obsolete("AppId Auth Backend has been deprecated from Vault as of Version version 0.6.1")]
        public static void EnableAppId(this string vaultUri, string rootToken)
        {
            using (var client = new JsonServiceClient(vaultUri))
            {
                client.AddHeader("X-Vault-Token", rootToken);
                client.Post<JsonObject>("v1/sys/auth/app-id", new { type = "app-id" });
            }
        }

        public static void CreatePolicy(this string vaultUri, string rootToken, string name, string path, string[] capabilities)
        {
            using (var client = new JsonServiceClient(vaultUri))
            {
                var rules = new { rules = CreateRule(path, capabilities).ToJson() }.ToJson();

                client.AddHeader("X-Vault-Token", rootToken);
                client.Put<string>($"v1/sys/policy/{name}", rules);
            }
        }

        private static JsonObject CreateRule(string path, string[] capabilities)
        {
            return new JsonObject
            {
                ["path"] = new JsonObject
                {
                    [path] = new JsonObject
                    {
                        ["capabilities"] = capabilities.ToJson()
                    }.ToJson()
                }.ToJson()
            };
        }

        [Obsolete("AppId Auth Backend has been deprecated from Vault as of Version version 0.6.1")]
        public static void CreateAppId(this string vaultUri, string rootToken, string appId, string policy)
        {
            using (var client = new JsonServiceClient(vaultUri))
            {
                client.AddHeader("X-Vault-Token", rootToken);

                client.Put<string>($"v1/auth/app-id/map/app-id/{appId}", new { value = policy });
            }
        }

        [Obsolete("AppId Auth Backend has been deprecated from Vault as of Version version 0.6.1")]
        public static void CreateUserId(this string vaultUri, string rootToken, string userId)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(vaultUri) })
            {
                client.DefaultRequestHeaders.Add("X-Vault-Token", rootToken);
                var response = client.PutAsync($"v1/auth/app-id/map/user-id/{userId}", null).Result;
            }
        }

        [Obsolete("AppId Auth Backend has been deprecated from Vault as of Version version 0.6.1")]
        public static void MapUserIdsToAppIds(this string vaultUri, string rootToken, string userId, params string[] appIds)
        {
            if (appIds == null || appIds.Length == 0)
                throw new Exception("user-id needs to be associated with at least 1 app-id");

            using (var client = new JsonServiceClient(vaultUri))
            {
                client.AddHeader("X-Vault-Token", rootToken);
                client.Post<JsonObject>($"v1/auth/app-id/map/user-id/{userId}", new { value = appIds.Join(",") });
            }
        }

        public static void EnableAppRole(this string vaultUri, string rootToken)
        {
            using (var client = new JsonServiceClient(vaultUri))
            {
                client.AddHeader("X-Vault-Token", rootToken);
                client.Post<JsonObject>("v1/sys/auth/approle", new { type = "approle" });
            }
        }

        public static void CreateAppRole(this string vaultUri, string rootToken, string appRole, string[] policies)
        {
            using (var client = new JsonServiceClient(vaultUri))
            {
                client.AddHeader("X-Vault-Token", rootToken);
                client.Post<JsonObject>($"v1/auth/approle/role/{appRole}", new { policies = string.Join(",", policies) });
            }
        }

        public static string GetAppRoleId(this string vaultUri, string rootToken, string appRole)
        {
            using (var client = new JsonServiceClient(vaultUri))
            {
                client.AddHeader("X-Vault-Token", rootToken);
                var response = client.Get<JsonObject>($"v1/auth/approle/role/{appRole}/role-id");
                var data = response.Get<JsonObject>("data");
                return data["role_id"];
            }
        }

        public static string GetAppRoleSecretId(this string vaultUri, string rootToken, string appRole)
        {
            using (var client = new JsonServiceClient(vaultUri))
            {
                client.AddHeader("X-Vault-Token", rootToken);

                var secretUrl = $"v1/auth/approle/role/{appRole}/secret-id";
                var response = client.Post<JsonObject>(secretUrl, null);
                var data = response.Get<JsonObject>("data");
                return data["secret_id"];
            }
        }
    }
}
