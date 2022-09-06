
using Archival.Application.Services.Responses;
using Archival.Core.Configuration;
using Archival.Core.Interfaces;
using Archival.Core.Model;
using Archival.Core.Model.Responses;
using Docman;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Archival.Application.Services
{
    public class CSAccess<T> : ICSAccess<T> where T : IRecord
    {
        private readonly BaseConfiguration _config;
        private readonly IAuthenticate _auth;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogManagerCustom _log;
        private readonly IUtil<T> _util;
        private readonly IRepository<T> _repository;
        public CSAccess(IOptions<BaseConfiguration> config, IRepository
            <T> repository, IHttpClientFactory httpClientFactory, IAuthenticate auth, ILogManagerCustom log, IUtil<T> util)
        {
            _httpClientFactory = httpClientFactory;
            _config = config.Value;
            _auth = auth;
            _log = log;
            _util = util;
            _repository = repository;
        }

        public async Task<NodeResponse> GetNode(long id)
        {
            string ticket = await _auth.Auth();
            var url = _config.Destination_URL + "api/v1/nodes/" + id;
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequestMessage.Headers.Add("OTCSTicket", ticket);

            var httpClientFactory = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClientFactory.SendAsync(httpRequestMessage);
            try
            {
                var result = await httpResponseMessage.Content.ReadAsStringAsync();
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var temp = await httpResponseMessage.Content.ReadAsStringAsync();
                    CSNode cSNode = JsonConvert.DeserializeObject<CSNode>(temp);
                    return new NodeResponse(cSNode);
                } 
            }
            catch (Exception e)
            {
                _log.debug(e.Message.ToString());
            }
            return new NodeResponse(httpResponseMessage.RequestMessage.ToString());
        }

        public async Task<NodeResponse> GetUser(long id)
        {
            string ticket = await _auth.Auth();
            var url = _config.Destination_URL + "api/v1/members/" + id;
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequestMessage.Headers.Add("OTCSTicket", ticket);

            var httpClientFactory = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClientFactory.SendAsync(httpRequestMessage);
            try
            {
                var result = await httpResponseMessage.Content.ReadAsStringAsync();
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var temp = await httpResponseMessage.Content.ReadAsStringAsync();
                    CSNode cSNode = JsonConvert.DeserializeObject<CSNode>(temp);
                    return new NodeResponse(cSNode);
                }
            }
            catch (Exception e)
            {
                _log.debug(e.Message.ToString());
            }
            return new NodeResponse(httpResponseMessage.RequestMessage.ToString());
        }
        public async Task<CaseResponse> GetCaseCategory(long id)
        {
            string ticket = await _auth.Auth();
            var url = _config.Destination_URL + "api/v1/nodes/" + id + "/categories/" + _config.CaseCategoryID;
            var client = new RestClient(url);
            var request = new RestRequest("", Method.Get);
            request.AddHeader("OTCSTicket", ticket);
            request.AlwaysMultipartFormData = true;
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                JObject obj = JObject.Parse(response.Content); 
                CaseCategory caseCategory = new CaseCategory();
                caseCategory.CaseCategory_CaseID = (string)obj["data"][_config.CaseCategory_CaseID.ToString()];
                caseCategory.CaseCategory_CaseTitle = (string)obj["data"][_config.CaseCategory_CaseTitle.ToString()];
                caseCategory.CaseCategory_OpenDate = (string)obj["data"][_config.CaseCategory_OpenDate.ToString()];
                caseCategory.CaseCategory_CloseDate = (string)obj["data"][_config.CaseCategory_CloseDate.ToString()];
                caseCategory.CaseCategory_Author = (string)obj["data"][_config.CaseCategory_Author.ToString()];
               
                return new CaseResponse(caseCategory);
            }
            return new CaseResponse(response.ErrorMessage);
        }
        public async Task<DocResponse> GetDocCategory(long id)
        {
            string ticket = await _auth.Auth();
            var url = _config.Destination_URL + "api/v1/nodes/" + id + "/categories/" + _config.DocumentCategoryID;
            var client = new RestClient(url);
            var request = new RestRequest("", Method.Get);
            request.AddHeader("OTCSTicket", ticket);
            request.AlwaysMultipartFormData = true;
            var response = await client.ExecuteAsync(request);


            if (response.IsSuccessful)
            {
                JObject obj = JObject.Parse(response.Content);
                DocCategory docCategory = new DocCategory();
                docCategory.DocumentCategory_DocDesc = (string)obj["data"][_config.DocumentCategory_DocDesc.ToString()];
                docCategory.DocumentCategory_PrevDocNo = (string)obj["data"][_config.DocumentCategory_PrevDocNo.ToString()];
                docCategory.DocumentCategory_SecurityClassif = (string)obj["data"][_config.DocumentCategory_SecurityClassif.ToString()];
                docCategory.DocumentCategory_AuthNumber = (string)obj["data"][_config.DocumentCategory_AuthNumber.ToString()];

                return new DocResponse(docCategory);
            }
            return new DocResponse(response.ErrorMessage);
        }

        public async Task<DownloadResponse> DownloadRecord(T record)
        {
            string ticket = await _auth.Auth(); 
            var url = _config.Destination_URL + "api/v1/nodes/" + record.dataID + "/content?action=download";
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            httpRequestMessage.Headers.Add("OTCSTicket", ticket);

            var httpClientFactory = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClientFactory.SendAsync(httpRequestMessage);
            try
            {
                var result = await httpResponseMessage.Content.ReadAsStringAsync();
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    if(record.conversionType == "MAIN_NAS" || record.conversionType == "MAIN_A")
                    {
                        if (record.convertedRecord.dataID != record.dataID)
                        {
                            HideRecord(true, record.dataID);
                        }
                        else
                        {
                            HideRecord(false, record.convertedRecord.dataID);
                        }
                    }

                    using (Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync())
                    {
                        string path = _util.GetBufferPath(record);
                        string destPath = _util.GetBufferPath(record, isPDF : true);

                        using (Stream writeTo = File.Open(path, FileMode.Create))
                        {
                            await stream.CopyToAsync(writeTo);
                        } 
                        return new DownloadResponse(new DownloadedRecord { sourcePath = path, destPath = destPath, fileName = record.name, conversionType = record.conversionType });
                    } 
                } 
            }
            catch (Exception e)
            { 
                return new DownloadResponse(e.Message.ToString());
            }
            return new DownloadResponse(httpResponseMessage.RequestMessage.ToString());
        }

        public async Task<string> UploadFile(T record)
        { 
            string result = "";
            try
            {
                string token = await _auth.Auth();
                string url = _config.Destination_URL + "api/v1/nodes";
                var client = new RestClient(url);
                var request = new RestRequest("", Method.Post);
                request.AddHeader("OTCSTicket", token);
                request.AddParameter("parent_id", record.parentID);
                request.AddParameter("type", "144");
                request.AddParameter("name", Path.GetFileName(record.convertedPath));
                request.AddFile("file", record.convertedPath);
                var response = await client.ExecuteAsync(request);
                var content = response.Content;

                if (content != null)
                {
                    dynamic dynamicResult = JObject.Parse(content);
                    result = dynamicResult.id;
                    long convertedID = long.Parse(result);
                    await SetPermission(convertedID);
                    await HideRecord(true, convertedID);
                    await _repository.AddEkrisRelated(record.dataID, convertedID);
                    await _repository.AddEkrisRelated(convertedID, record.dataID);
                }
            }
            catch (Exception e)
            {
                _log.debug(e.Message.ToString());
            }
            return result;
        }

        public async Task UpdateDocCategory(long id)
        {
            string token = await _auth.Auth();
            string url = _config.Destination_URL + "api/v2/nodes" + id + "/categories/" + _config.DocumentCategoryID;
            var client = new RestClient(url);
            var request = new RestRequest("", Method.Put);
            request.AddHeader("OTCSTicket", token);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter(_config.DocumentCategory_PrevDocNo, id);
            var response = await client.ExecuteAsync(request); 
        }

        public async Task AddVersion(T record)
        {
            string token = await _auth.Auth();

            string url = _config.Destination_URL + "api/v2/nodes/" + record.convertedRecord.dataID + "/versions/";
            var client = new RestClient(url);
            var request = new RestRequest("", Method.Post);
            request.AddHeader("OTCSTicket", token);
            request.AddFile("file", record.convertedPath);
            var response = await client.ExecuteAsync(request); 
        }
        public async Task HideRecord(bool hide, long convertedID)
        {
            string token = await _auth.Auth();

            string url = _config.Destination_URL + "api/v1/nodes/" + convertedID;
            var client = new RestClient(url);
            var request = new RestRequest("", Method.Put);
            request.AlwaysMultipartFormData = true;
            request.AddHeader("OTCSTicket", token);
            request.AddParameter("hidden", hide);
            request.AddParameter("description", "");
            var response = await client.ExecuteAsync(request);
        }
        //temp
        /*
        public async Task HideRecord(bool hide, long convertedID)
        {
            string token = await _auth.AuthenticateCWS();
            Docman.OTAuthentication auth = new Docman.OTAuthentication();
            auth.AuthenticationToken = token;
            try
            {
                using (DocumentManagementClient docman = new DocumentManagementClient())
                {
                    var node = await docman.GetNodeAsync(auth, convertedID);
                    if (hide) node.GetNodeResult.Catalog = 2;
                    else node.GetNodeResult.Catalog = 0;

                    await docman.UpdateNodeAsync(auth, node.GetNodeResult);
                }
            }
            catch(Exception e)
            {

            }
        } 
        public async Task SetPermission(long id)
        {
            string token = await _auth.AuthenticateCWS();
            NodeRights permission = new NodeRights();
            using (DocumentManagementClient docman = new DocumentManagementClient(DocumentManagementClient.EndpointConfiguration.BasicHttpBinding_DocumentManagement))
            {
                try
                {
                    string endpoint = _config.docmanSetting.Endpoint;
                    docman.Endpoint.Address = new EndpointAddress(endpoint);

                    Docman.OTAuthentication auth = new Docman.OTAuthentication();
                    auth.AuthenticationToken = token;

                    var nodeRights = await docman.GetNodeRightsAsync(auth, id);

                    if (nodeRights.GetNodeRightsResult != null)
                    {
                        permission = nodeRights.GetNodeRightsResult;

                        //Owner Rights
                        if (permission.OwnerRight != null)
                        {
                            permission.OwnerRight.Permissions.AddItemsPermission = false;
                            permission.OwnerRight.Permissions.DeletePermission = false;
                            permission.OwnerRight.Permissions.DeleteVersionsPermission = false;
                            permission.OwnerRight.Permissions.EditAttributesPermission = false;
                            permission.OwnerRight.Permissions.EditPermissionsPermission = false;
                            permission.OwnerRight.Permissions.ModifyPermission = false;
                            permission.OwnerRight.Permissions.ReservePermission = true;
                        }

                        //Owner Group Rights
                        if (permission.OwnerGroupRight != null)
                        {
                            permission.OwnerGroupRight.Permissions.DeletePermission = false;
                            permission.OwnerGroupRight.Permissions.DeleteVersionsPermission = false;
                            permission.OwnerGroupRight.Permissions.ModifyPermission = false;
                            permission.OwnerGroupRight.Permissions.AddItemsPermission = false;
                            permission.OwnerGroupRight.Permissions.ReservePermission = true;
                            permission.OwnerGroupRight.Permissions.EditAttributesPermission = false;
                            permission.OwnerGroupRight.Permissions.EditPermissionsPermission = false;
                        }

                        //Public Rights
                        if (permission.PublicRight != null)
                        {
                            permission.PublicRight.Permissions.DeletePermission = false;
                            permission.PublicRight.Permissions.DeleteVersionsPermission = false;
                            permission.PublicRight.Permissions.ModifyPermission = false;
                            permission.PublicRight.Permissions.AddItemsPermission = false;
                            permission.PublicRight.Permissions.ReservePermission = true;
                            permission.PublicRight.Permissions.EditAttributesPermission = false;
                            permission.PublicRight.Permissions.EditPermissionsPermission = false;
                        }

                        //ACL Rights
                        if (permission.ACLRights != null)
                        {
                            foreach (NodeRight ACL in permission.ACLRights)
                            {
                                ACL.Permissions.DeletePermission = false;
                                ACL.Permissions.DeleteVersionsPermission = false;
                                ACL.Permissions.ModifyPermission = false;
                                ACL.Permissions.AddItemsPermission = false;
                                ACL.Permissions.ReservePermission = true;
                                ACL.Permissions.EditAttributesPermission = false;
                                ACL.Permissions.EditPermissionsPermission = false;
                            }
                        }
                    }

                    await docman.SetNodeRightsAsync(auth, id, permission);
                    docman.Close();
                }
                catch (Exception e)
                {
                    docman.Close();
                    _log.debug("SetPermission : " + e.Message.ToString());
                }
            }
        } 
      
        */
        private async Task SetPermission(long convertedID)
        {
            var permissions = await GetPermission(convertedID);
            await UpdatePermissions(permissions);
        }
        private async Task<List<ConvertedPermission>> GetPermission(long convertedID)
        {
            string token = await _auth.Auth();
            string url = _config.Destination_URL + "api/v2/nodes/" + convertedID + "/permissions";
            var client = new RestClient(url);
            var request = new RestRequest("", Method.Get);
            request.AddHeader("OTCSTicket", token);
            request.AlwaysMultipartFormData = true; 
            var response = await client.ExecuteAsync(request);
            var res = JsonConvert.DeserializeObject<Permissions>(response.Content);

            List<ConvertedPermission> data = new List<ConvertedPermission>();
            if (res.Results.Length > 0)
            {
                for (int i = 0; i < res.Results.Length; i++)
                {
                    ConvertedPermission temp = new ConvertedPermission();
                    long? tempID = res.Results[i].Data.Permissions.RightId;

                    if (tempID == null) continue;
                    temp.id = (long)tempID;
                    temp.type = res.Results[i].Data.Permissions.Type;
                    temp.convertedID = convertedID;
                    data.Add(temp);
                }
            }
            return data;
        }

        private async Task UpdatePermissions(List<ConvertedPermission> data)
        {
            for(int i=0;i<data.Count();i++)
            {
                long rightID = data[i].id;
                await UpdatePermission(data[i].convertedID, rightID, data[i].type);
            }
        }

        private async Task UpdatePermission(long convertedID, long rightID, string type)
        {
            if (type == "custom")
            {
                type += "/" + rightID;
            }

            string token = await _auth.Auth();
            string url = _config.Destination_URL + "api/v2/nodes/" + convertedID + "/permissions/" +type;
            var client = new RestClient(url);
            var request = new RestRequest("", Method.Put);
            request.AddHeader("OTCSTicket", token);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("body", "{\"permissions\":[\"see\",\"see_contents\",\"modify\",\"reserve\"],\"include_sub_types\":[]}");
            var response = await client.ExecuteAsync(request);
        } 
    }
}
