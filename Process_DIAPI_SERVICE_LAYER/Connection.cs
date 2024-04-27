using SAPbobsCOM;
using System.Windows.Forms;
using System;
using RestSharp;
using Process_DIAPI_SERVICE_LAYER.Models;
using Newtonsoft.Json;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

namespace Process_DIAPI_SERVICE_LAYER
{
    public class Connection
    {
        public static Company DiCompany { get; private set; } = new Company();

        public static string SLSession { get; private set; } = "";

        public async Task<bool> Success(string server, string companyDB, string userName, string password, string url)
        {
            if (!DiCompany.Connected)
            {
                if (diConn(server, companyDB, userName, password))
                    return await SLConn(companyDB, userName, password, url);
                else return false;
            }
            else
            {
                return true;
            }

        }

        private bool diConn(string server, string companyDB, string userName, string password)
        {
            try
            {

                DiCompany.SLDServer = server + ":40000";
                DiCompany.Server = server;
                DiCompany.DbServerType = BoDataServerTypes.dst_MSSQL2019;
                DiCompany.CompanyDB = companyDB;
                DiCompany.UserName = userName;
                DiCompany.Password = password;
                DiCompany.language = BoSuppLangs.ln_Portuguese_Br;

                int res = DiCompany.Connect();

                if (res != 0)
                {

                    throw new Exception($"Error DIAPI: ({DiCompany.GetLastErrorCode()}) - {DiCompany.GetLastErrorDescription()}");
                }

                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private async Task<bool> SLConn(string companyDB, string userName, string password, string url)
        {
            try
            {

                Login login = new Login();

                login.UserName = userName;
                login.Password = password;
                login.CompanyDB = companyDB;
                login.Language = "ln_Portuguese_Br";

                ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };

                var client = new RestClient(url);
                var request = new RestRequest("/Login", Method.POST);

                var body = JsonConvert.SerializeObject(login);

                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", body, ParameterType.RequestBody);

                RestResponse response = (RestResponse)await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    SLSession = response.Cookies.FirstOrDefault(x => x.Name == "B1SESSION").Value;
                    return true;
                }
                else
                {
                    DiCompany.Disconnect();
                    ErrorSL er = JsonConvert.DeserializeObject<ErrorSL>(response.Content);
                    throw new Exception($"Error SL: ({er.error.code}) - {er.error.message.value}");
                }

            }
            catch (Exception ex)
            {
                if (DiCompany.Connected)
                {
                    DiCompany.Disconnect();
                }
                MessageBox.Show(ex.Message);
                return false;
            }

        }

    }

}




