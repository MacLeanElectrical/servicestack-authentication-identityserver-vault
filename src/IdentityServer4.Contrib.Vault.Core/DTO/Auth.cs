// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Vault.Core.DTO
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class Auth
    {
        [JsonProperty("client_token")]
        public string ClientToken { get; set; }

        [JsonProperty("accessor")]
        public string Accessor { get; set; }

        [JsonProperty("policies")]
        public string[] Policies { get; set; }

        [JsonProperty("metadata")]
        public IDictionary<string, string> Metadata { get; set; }

        [JsonProperty("lease_duration")]
        public int LeaseDuration { get; set; }

        [JsonProperty("renewable")]
        public bool Renewable { get; set; }
    }
}