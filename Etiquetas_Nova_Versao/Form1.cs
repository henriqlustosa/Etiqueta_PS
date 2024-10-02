using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Drawing.Printing;

namespace Etiquetas_Nova_Versao
{
    public partial class Form1 : Form
    {
       

        int status;
        Paciente paciente;
       // Internacao internacao;
      //  List<Internacao> internacoes;
        string error;
        //string Andar ="";
        ///string Quarto ="";
        //string Leito ="";
       // string Clinica="";
        int starty = 10;//distancia das linhas



        int startXEsquerda = 15;
     
        public class Paciente
        {
            public string nm_consulta { get; set; }
            public string data_consulta { get; set; }

            public string nm_paciente { get; set; }
            public string data_nascimento { get; set; }
            public string sexo { get; set; }
            public string nm_mae_paciente { get; set; }
           
        }
      /*  public class Internacao
        {
            public string cd_prontuario { get; set; }
            public string nr_leito { get; set; }
            public string dt_alta_medica { get; set; }
            public string nm_especialidade { get; set; }

        }*/
        public Form1()
        {
            InitializeComponent();

         
        }



        private void btImprimir_Click(object sender, EventArgs e)
        {
            try
            {

                string rh = txbRh.Text;
                //detiq = dados.getDados(be);
                string url = "http://10.48.21.64:5003/hspmsgh-api/pacientes/consulta/" + rh;
                WebRequest request = WebRequest.Create(url);
                try
                {
                    using (var twitpicResponse = (HttpWebResponse)request.GetResponse())
                    {
                        using (var reader = new StreamReader(twitpicResponse.GetResponseStream()))
                        {
                            JsonSerializer json = new JsonSerializer();
                            var objText = reader.ReadToEnd();
                            paciente = JsonConvert.DeserializeObject<Paciente>(objText);

                        }
                    }

                /*    string url2 = "http://10.48.21.64:5003/hspmsgh-api/internacoes/" + rh;
                    WebRequest request2 = WebRequest.Create(url2);
                    using (var twitpicResponse2 = (HttpWebResponse)request2.GetResponse())
                    {
                        using (var reader2 = new StreamReader(twitpicResponse2.GetResponseStream()))
                        {
                            JsonSerializer json2 = new JsonSerializer();
                            var objText2 = reader2.ReadToEnd();
                            internacoes = JsonConvert.DeserializeObject<List<Internacao>>(objText2);

                        }
                    }

                    if (internacoes.Count != 0)
                    {
                        if (internacoes[0].dt_alta_medica == null)

                        Andar = internacoes[0].nr_leito == null ? "" : internacoes[0].nr_leito.Substring(0, 2);
                        Quarto = internacoes[0].nr_leito == null ? "" : internacoes[0].nr_leito.Substring(2, 2);
                        Leito = internacoes[0].nr_leito == null ? "" : internacoes[0].nr_leito.Substring(5, 2);
                        Clinica = internacoes[0].nm_especialidade == null ? "" : internacoes[0].nm_especialidade;


                    }

                    */

                   // PrintDialog printDialog1 = new PrintDialog();
//printDialog1.Document = printDocument1;  
                  
                    printDocument1.PrinterSettings.PrinterName = "ImpressoraPS";
                  
                   
                        
                    { 
                        
                           
                        
                          
                        int i = 0;
                        int size =Convert.ToInt32( comboBox1.SelectedItem);
                        
                            while (i < size)
                            {
                                printDocument1.Print();
                                i++;
                            }
                     
                    }


                    txbRh.Text = "";
                  //  Andar = "";
                  //  Quarto = "";
                   // Leito = "";
                   // Clinica = "";



            }
                catch (Exception ex)
                {
                    MessageBox.Show("Número de RH inexistente! " + ex.Message);
                    status = 1;

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Número de RH inexistente! " + ex.Message);
                status = 1;

            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btImprimir.Enabled = true;
            if (status == 1)
                lblError.Text = error;
            else
                lblError.ResetText();



        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }



        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            string nomep = "";
            string nomeCompos = "";
            string nomem = "";
            string nomeComposM = "";
            string nm_nome = paciente.nm_paciente;
            string nm_nome_mae = paciente.nm_mae_paciente;
            

            if (paciente.nm_paciente.Length > 20)
            {
             nomep = nm_nome.Substring(0, 20);
             nomeCompos = nm_nome.Substring(20);
            }

            if (paciente.nm_mae_paciente.Length > 18)
            {
                nomem = nm_nome_mae.Substring(0, 18);
                nomeComposM = nm_nome_mae.Substring(18);
            }


            //DateTime dt = DateTime.Now;
            //string data = dt.ToString("g");  

            e.PageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 25, 100);//900 é a largura da página
            //printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 500, 1000);
           // printDialog1.PrinterSettings.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize("Custom2", 500, 1000);
            Graphics g = e.Graphics;
           
           


            

                   

                    if (paciente.nm_paciente.Length > 20)
                    {




                      
                        g.DrawString("Nº BE: " + paciente.nm_consulta , new Font("Arial", 8, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty );
                        g.DrawString("Data: " + paciente.data_consulta, new Font("Arial", 8, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty +15);
                        g.DrawString("Nome: " + nomep, new Font("Arial", 8, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 30);
                        
                        
                        g.DrawString( nomeCompos, new Font("Arial", 8, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 45);
                      
                       g.DrawString("Nasc: " + paciente.data_nascimento    + "  Sexo: " + paciente.sexo, new Font("Arial", 8 , FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 60);




                       if (paciente.nm_mae_paciente.Length > 20)
                       {
                           g.DrawString("Filiação: " + nomem, new Font("Arial", 8, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 75);


                           g.DrawString(nomeComposM, new Font("Arial", 8, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 90);
                       }
                       else {

                           g.DrawString("Filiação: " + paciente.nm_mae_paciente, new Font("Arial", 8, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 75);

                       }

                      


                      

                    }
                    else
                    {
                       
                        g.DrawString("BE: " + paciente.nm_consulta , new Font("Arial", 8, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty );
                        g.DrawString("Data: " + paciente.data_consulta, new Font("Arial", 8, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 15);
                        g.DrawString("Nome: " + paciente.nm_paciente, new Font("Arial", 8, FontStyle.Bold), System.Drawing.Brushes.Black, startXEsquerda, starty + 30);

                      

                        g.DrawString("Nasc: " + paciente.data_nascimento + "     Sexo: " + paciente.sexo, new Font("Arial", 8, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 45);

                        if (paciente.nm_mae_paciente.Length > 20)
                        {
                            g.DrawString("Filiação: " + nomem, new Font("Arial", 8, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 60);


                            g.DrawString(nomeComposM, new Font("Arial", 8, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 75);
                        }
                        else
                        {

                            g.DrawString("Filiação: " + paciente.nm_mae_paciente, new Font("Arial", 8, FontStyle.Regular), System.Drawing.Brushes.Black, startXEsquerda, starty + 60);

                        }

                      
                     


                  
                    }
                 
            

            


        }

        private void txbRh_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == (char)Keys.Enter)
            {

                btImprimir_Click(sender, e);

            }
        }
    }
}