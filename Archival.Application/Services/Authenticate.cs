using Archival.Core.Configuration;
using Archival.Core.Interfaces;
using Auth;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Application.Services
{
    public class Authenticate : IAuthenticate
    {
        private readonly BaseConfiguration _config;
        private readonly ILogManagerCustom _log;
        private readonly ISecureInfo _secure;
        public Authenticate(IOptions<BaseConfiguration> config, ILogManagerCustom log, ISecureInfo secure)
        {
            _config = config.Value;
            _log = log;
            _secure = secure;
        }

        public async Task<string> Auth()
        {
            string authToken = "";
            bool success = false;
            int attemptCount = 1;
            int noOfAttempts = 2;
            while (success == false && attemptCount <= noOfAttempts)
            {
                try
                {
                    string OTCS_loginId = "", OTCS_password = "";
                    if (_config.UseSecureCredentials)
                    {
                        OTCS_loginId = _secure.getSensitiveInfo(_config.secureInfo.fileName.CSUsername);
                        OTCS_password = _secure.getSensitiveInfo(_config.secureInfo.fileName.CSPassword);
                    }
                    else
                    {
                        OTCS_loginId = _config.Username;
                        OTCS_password = _config.Password;
                    }

                    string url = _config.Destination_URL + "api/v1/auth";
                    HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, url);
                    Dictionary<string, string> formData = new Dictionary<string, string>();
                    formData.Add("username", OTCS_loginId);
                    formData.Add("password", OTCS_password);

                    httpRequestMessage.Content = new FormUrlEncodedContent(formData);
                    HttpClient httpClient = new HttpClient();
                    var response = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead);
                    try
                    {
                        string res = await response.Content.ReadAsStringAsync();
                        dynamic dynamicResult = JObject.Parse(res);
                        authToken = dynamicResult.ticket;
                        success = true;
                    }
                    catch (Exception e)
                    {
                        _log.debug(e.Message.ToString());
                    }
                }
                catch (Exception e)
                {
                    _log.debug("Failed to get token with total attempt " + attemptCount + " of " + noOfAttempts + " " + e.Message.ToString());
                    attemptCount++;
                }
            }
            return authToken;
        }

        //temp
        public async Task<string> AuthenticateCWS()
        {
            string authToken = "";
            bool success = false;
            int attemptCount = 1;
            int noOfAttempts = 2;
            while (success == false && attemptCount <= noOfAttempts)
            {
                try
                {
                    using (AuthenticationClient authClient = new AuthenticationClient())
                    {
                        try
                        {
                            string OTCS_loginId = "", OTCS_password = "";
                            if (_config.UseSecureCredentials)
                            {
                                OTCS_loginId = _secure.getSensitiveInfo(_config.secureInfo.fileName.CSUsername);
                                OTCS_password = _secure.getSensitiveInfo(_config.secureInfo.fileName.CSPassword);
                            }
                            else
                            {
                                OTCS_loginId = _config.Username;
                                OTCS_password = _config.Password;
                            }

                            Auth.OTAuthentication otAuthAdmin = new Auth.OTAuthentication();
                            authToken = await authClient.AuthenticateUserAsync(OTCS_loginId, OTCS_password);
                            OTCS_password = null;//clear from memory
                            authClient.Close();
                            success = true;
                            _log.debug("authenticated with CWS as " + OTCS_loginId);
                            authClient.Close();
                        }
                        catch (Exception e)
                        {
                            authClient.Close();
                            throw e;
                        }
                    }
                }
                catch (Exception e)
                {
                    _log.debug("failed to get token: attempt " + attemptCount + " of " + noOfAttempts);

                    if (attemptCount >= noOfAttempts)
                    {
                        throw e;
                    }
                    attemptCount++;
                }
            }
            return authToken;
        }
    }
}
