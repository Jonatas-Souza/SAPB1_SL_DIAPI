using Newtonsoft.Json;
using Process_DIAPI_SERVICE_LAYER.Models;
using RestSharp;
using SAPbobsCOM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Process_DIAPI_SERVICE_LAYER
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void label17_Click(object sender, EventArgs e)
        {

        }

        private async void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                Connection conn = new Connection();
                bool res = await conn.Success(textServer.Text, textCompany.Text, textUser.Text, textPassword.Text, textURL.Text);

                int cnt = 0;

                if (res)
                {
                    textDIAPIStatus.Text = "Connected";
                    textSLSession.Text = Connection.SLSession;

                    if (cnt == 0)
                    {
                        List<KeyValuePair<int, string>> PNs = new List<KeyValuePair<int, string>>();
                        List<KeyValuePair<int, string>> Items = new List<KeyValuePair<int, string>>();

                        string grpIt = $@"select ""ItmsGrpCod"",""ItmsGrpNam"" from OITB",
                               grpPn = $@"select ""GroupCode"",""GroupName"" from OCRG WHERE ""GroupType"" = 'C'";

                        SAPbobsCOM.Recordset rs = Connection.DiCompany.GetBusinessObject(BoObjectTypes.BoRecordset);

                        rs.DoQuery(grpPn);


                        if (rs.RecordCount > 0)
                        {
                            for (int i = 1; i <= rs.RecordCount; i++)
                            {
                                PNs.Add(new KeyValuePair<int, string>((int)rs.Fields.Item("GroupCode").Value, (string)rs.Fields.Item("GroupName").Value));

                                if (i < rs.RecordCount)
                                {
                                    rs.MoveNext();
                                }
                            }

                            comboBP.DataSource = new BindingSource(PNs, null);
                            comboBP.DisplayMember = "Value";
                            comboBP.ValueMember = "Key";
                        }

                        rs.DoQuery(grpIt);

                        if (rs.RecordCount > 0)
                        {
                            for (int i = 1; i <= rs.RecordCount; i++)
                            {
                                Items.Add(new KeyValuePair<int, string>((int)rs.Fields.Item("ItmsGrpCod").Value, (string)rs.Fields.Item("ItmsGrpNam").Value));

                                if (i < rs.RecordCount)
                                {
                                    rs.MoveNext();
                                }
                            }

                            comboItems.DataSource = new BindingSource(Items, null);
                            comboItems.DisplayMember = "Value";
                            comboItems.ValueMember = "Key";
                        }

                        System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);

                        cnt++;
                    }

                }
                else
                {
                    textDIAPIStatus.Text = "Disconnected";
                    textSLSession.Text = "";
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void buttonBP_Click(object sender, EventArgs e)
        {

            try
            {
                SAPbobsCOM.BusinessPartners bp = Connection.DiCompany.GetBusinessObject(BoObjectTypes.oBusinessPartners);

                bp.CardCode = textCardCode.Text;
                bp.CardName = textCardName.Text;
                bp.GroupCode = (int)comboBP.SelectedValue;

                int ret = bp.Add();

                if (ret != 0)
                {
                    throw new Exception($"Error DIAPI: ({Connection.DiCompany.GetLastErrorCode()}) - {Connection.DiCompany.GetLastErrorDescription()}");
                }

                MessageBox.Show($"PN {bp.CardCode} cadastrado com sucesso!");

                getPN(bp.CardCode);

                System.Runtime.InteropServices.Marshal.ReleaseComObject(bp);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

        private async void getPN(string cardCode)
        {
            try
            {

                string url = textURL.Text;

                var client = new RestClient(url);
                var request = new RestRequest($"/BusinessPartners('{cardCode}')", Method.GET);

                CookieContainer cookiecon = new CookieContainer();
                cookiecon.Add(new Cookie("B1SESSION", Connection.SLSession, "/b1s/v1", textServer.Text));

                client.CookieContainer = cookiecon;

                RestResponse response = (RestResponse)await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    BPmodel bp = JsonConvert.DeserializeObject<BPmodel>(response.Content);

                    textSalesBPCode.Text = bp.CardCode;
                    textSaleBPName.Text = bp.CardName;
                }
                else
                {
                    ErrorSL er = JsonConvert.DeserializeObject<ErrorSL>(response.Content);
                    throw new Exception($"Error SL: ({er.error.code}) - {er.error.message.value}");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void buttonItems_Click(object sender, EventArgs e)
        {
            try
            {
                SAPbobsCOM.Items it = Connection.DiCompany.GetBusinessObject(BoObjectTypes.oItems);

                it.ItemCode = textItemCode.Text;
                it.ItemName = textItemName.Text;
                it.ItemsGroupCode = (int)comboItems.SelectedValue;

                int ret = it.Add();

                if (ret != 0)
                {
                    throw new Exception($"Error DIAPI: ({Connection.DiCompany.GetLastErrorCode()}) - {Connection.DiCompany.GetLastErrorDescription()}");
                }

                MessageBox.Show($"Item {it.ItemCode} cadastrado com sucesso!");

                getITEM(it.ItemCode);

                System.Runtime.InteropServices.Marshal.ReleaseComObject(it);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private async void getITEM(string itemCode)
        {
            try
            {

                string url = textURL.Text;

                var client = new RestClient(url);
                var request = new RestRequest($"/Items('{itemCode}')", Method.GET);

                CookieContainer cookiecon = new CookieContainer();
                cookiecon.Add(new Cookie("B1SESSION", Connection.SLSession, "/b1s/v1", textServer.Text));

                client.CookieContainer = cookiecon;

                RestResponse response = (RestResponse)await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    ItemModel item = JsonConvert.DeserializeObject<ItemModel>(response.Content);

                    int line = dataGridView1.RowCount - 1;

                    dataGridView1.Rows.Insert(line, item.ItemCode, item.ItemName, 1, "0,00", "0,00");

                }
                else
                {
                    ErrorSL er = JsonConvert.DeserializeObject<ErrorSL>(response.Content);
                    throw new Exception($"Error SL: ({er.error.code}) - {er.error.message.value}");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private async void buttonSales_Click(object sender, EventArgs e)
        {
            try
            {
                string url = textURL.Text;

                OrdersModel orders = new OrdersModel();

                orders.CardCode = textSalesBPCode.Text;
                orders.CardName = textSaleBPName.Text;
                orders.DocDueDate = DateTime.Now;
                orders.BPL_IDAssignedToInvoice = 1;
                orders.Comments = $"Pedido de venda gerado pela SL as {DateTime.Now}";

                orders.DocumentLines = new List<Orderline>();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row != null && !row.IsNewRow)
                    {
                        Orderline orderline = new Orderline
                        {
                            ItemCode = row.Cells["ItemCode"].Value.ToString(),
                            ItemDescription = row.Cells["ItemName"].Value.ToString(),
                            Quantity = Convert.ToDouble(row.Cells["Quantity"].Value),
                            UnitPrice = Convert.ToDouble(row.Cells["Price"].Value)
                        };

                        orders.DocumentLines.Add(orderline);
                    }
                }


                var client = new RestClient(url);
                var request = new RestRequest("/Orders", Method.POST);

                var body = JsonConvert.SerializeObject(orders);

                request.AddParameter("application/json", body, ParameterType.RequestBody);

                CookieContainer cookiecon = new CookieContainer();
                cookiecon.Add(new Cookie("B1SESSION", Connection.SLSession, "/b1s/v1", textServer.Text));

                client.CookieContainer = cookiecon;

                RestResponse response = (RestResponse)await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    OrdersModel pv = JsonConvert.DeserializeObject<OrdersModel>(response.Content);

                    textSaledDocNum.Text = pv.DocNum.ToString();

                    MessageBox.Show($"Pedido de venda {pv.DocNum} gerado com sucesso!");

                }
                else
                {
                    ErrorSL er = JsonConvert.DeserializeObject<ErrorSL>(response.Content);
                    throw new Exception($"Error SL: ({er.error.code}) - {er.error.message.value}");
                }


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }


        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && (e.ColumnIndex == 2 || e.ColumnIndex == 3))
            {
                double preco, quantidade, total;

                if (dataGridView1.Rows[e.RowIndex].Cells["Price"].Value != null)
                {
                    preco = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells["Price"].Value);
                }
                else
                {
                    dataGridView1.Rows[e.RowIndex].Cells["Price"].Value = "0,00";
                    preco = 0;
                }

                if (dataGridView1.Rows[e.RowIndex].Cells["Quantity"].Value != null)
                {
                    quantidade = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells["Quantity"].Value);
                }
                else
                {
                    dataGridView1.Rows[e.RowIndex].Cells["Quantity"].Value = "0,00";
                    quantidade = 0;
                }

                total = quantidade * preco;

                dataGridView1.Rows[e.RowIndex].Cells["Total"].Value = total.ToString(CultureInfo.InvariantCulture);

            }
        }


    }
}
