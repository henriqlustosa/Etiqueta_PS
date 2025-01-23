using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Drawing.Printing;

namespace Etiquetas_Nova_Versao
{
    public partial class Form1 : Form
    {
        private int status;
        private Paciente paciente;
        private string error;

        // Distância inicial das linhas (top padding).
        private const int START_Y = 5;
        private const int START_X = 10;

        public Form1()
        {
            InitializeComponent();
        }

        public class Paciente
        {
            public string nm_consulta { get; set; }
            public string check_in { get; set; }
            public string nm_social { get; set; }
            public string nm_paciente { get; set; }
            public string data_nascimento { get; set; }
            public string sexo { get; set; }
            public string nm_mae_paciente { get; set; }
            public string nm_pai_paciente { get; set; }
        }

        #region Button Events

        private void btImprimir_Click(object sender, EventArgs e)
        {
            string rh = txbRh.Text.Trim();

            // .NET 3.5 doesn't have IsNullOrWhiteSpace:
            if (string.IsNullOrEmpty(rh) || rh.Trim().Length == 0)
            {
                MessageBox.Show("Por favor, digite um número de RH válido.");
                return;
            }

            try
            {
                // 1. Busca dados do paciente
                paciente = FetchPacienteData(rh);

                if (paciente == null)
                {
                    // Caso a requisição retorne nulo ou não tenha encontrado o paciente
                    MessageBox.Show("Paciente com RH " + rh + " não encontrado.");
                    return;
                }

                // 2. Configura a impressora
                printDocument1.PrinterSettings.PrinterName = "EtiquetaPSI";

                // 3. Lê a quantidade de impressões desejadas
                object selectedItem = comboBox1.SelectedItem;
                if (selectedItem == null)
                {
                    MessageBox.Show("Selecione uma quantidade de etiquetas válida.");
                    return;
                }

                int size;
                // .NET 3.5 supports int.TryParse:
                if (!int.TryParse(selectedItem.ToString(), out size) || size <= 0)
                {
                    MessageBox.Show("Selecione uma quantidade de etiquetas válida.");
                    return;
                }

                // 4. Realiza a impressão múltipla
                for (int i = 0; i < size; i++)
                {
                    printDocument1.Print();
                }

                // 5. Limpa o RH
                txbRh.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro durante a operação: " + ex.Message);
                status = 1;
            }
        }

        #endregion

        #region BackgroundWorker Events (if needed)

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // Caso utilize o backgroundWorker no futuro
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btImprimir.Enabled = true;
            if (status == 1)
                lblError.Text = error;
            else
                lblError.ResetText();
        }

        #endregion

        #region Printing

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            // -- Paper size em centésimos de polegada (197 x 120 => 1.97" x 1.20") --
            e.PageSettings.PaperSize = new PaperSize("Custom2", 197, 120);

            Graphics g = e.Graphics;

            // Constrói o nome do paciente levando em conta o "nm_social"
            string nomeCompleto = BuildNomeCompleto(
                paciente.nm_paciente,
                paciente.nm_social
            );

            // Se "DESCONHECIDA", usa o nome do pai
            string nomeMae = (paciente.nm_mae_paciente == "DESCONHECIDA")
                ? paciente.nm_pai_paciente
                : paciente.nm_mae_paciente;

            // Desenha as linhas de texto
            float currentY = START_Y;

            // Linha 1
            currentY = DrawLine(g, "BE: " + paciente.nm_consulta, currentY, 8, false);

            // Linha 2
            currentY = DrawLine(g, "Data: " + paciente.check_in, currentY, 8, false);

            // Linha 3 e 4 (Nome paciente)
            // Nome pode ser grande, então tratamos em duas linhas se ultrapassar 20 caracteres
            currentY = DrawWrappedLine(g, "Nome: ", nomeCompleto, currentY);

            // Linha seguinte - Nascimento e Sexo
            currentY = DrawLine(g, "Nasc: " + paciente.data_nascimento + "  Sexo: " + paciente.sexo, currentY, 8, false);

            // Filiação
            currentY = DrawWrappedLine(g, "Filiação: ", nomeMae, currentY);
        }

        /// <summary>
        /// Desenha uma linha simples de texto e retorna a nova posição Y (abaixo da linha).
        /// </summary>
        private float DrawLine(Graphics g, string text, float currentY, int fontSize, bool isBold)
        {
            using (var font = new Font("Arial", fontSize, isBold ? FontStyle.Bold : FontStyle.Regular))
            {
                g.DrawString(text, font, Brushes.Black, START_X, currentY);
                // Avança ~15px para a próxima linha (pode ajustar conforme necessidade)
                currentY += 15f;
            }
            return currentY;
        }

        /// <summary>
        /// Desenha texto que pode precisar ser quebrado em duas linhas (ex.: Nome longo).
        /// Retorna a nova posição Y (abaixo da última linha desenhada).
        /// </summary>
        private float DrawWrappedLine(Graphics g, string prefix, string text, float currentY)
        {
            // Define o limite de 20 chars, como no seu código original
            const int LIMIT = 20;

            if (text.Length <= LIMIT)
            {
                // Pode caber em uma linha
                currentY = DrawLine(g, prefix + text, currentY, 8, textIsBold(prefix));
            }
            else
            {
                // Divide em duas linhas
                string firstPart = text.Substring(0, LIMIT);
                string secondPart = text.Substring(LIMIT);

                // Primeira linha (com prefixo)
                currentY = DrawLine(g, prefix + firstPart, currentY, 8, textIsBold(prefix));

                // Segunda linha (sem prefixo)
                currentY = DrawLine(g, secondPart, currentY, 8, textIsBold(prefix));
            }
            return currentY;
        }

        /// <summary>
        /// Decide se o texto deve ser negrito ou não, baseado no prefixo.
        /// (No código original, "Nome" era impresso em negrito, mas "Filiação" não necessariamente.)
        /// Ajuste conforme sua necessidade.
        /// </summary>
        private bool textIsBold(string prefix)
        {
            // Se o prefixo for "Nome: ", retorna true (negrito); senão false
            return prefix.IndexOf("Nome", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Faz a requisição web para buscar o paciente via JSON.
        /// </summary>
        private Paciente FetchPacienteData(string rh)
        {
            string url = "http://10.48.17.99:5003/hspmsgh-api/pacientes/consulta/" + rh;
            WebRequest request = WebRequest.Create(url);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string result = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<Paciente>(result);
            }
        }

        /// <summary>
        /// Combina o nome social com o nome do paciente, se existir.
        /// </summary>
        private string BuildNomeCompleto(string nome, string nomeSocial)
        {
            if (!string.IsNullOrEmpty(nomeSocial))
            {
                // Formato "Social (Original)"
                return nomeSocial + " (" + nome + ")";
            }
            return nome;
        }

        #endregion

        #region TextBox Events

        private void txbRh_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btImprimir_Click(sender, e);
            }
        }

        #endregion
    }
}
