using System;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;

namespace projeto1
{
    private StringBuilder bufferRecebido = new StringBuilder();
    public partial class Form1 : Form
    {
        private bool estadoLED = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void atualizaListaCOMs()
        {
            int i = 0;
            bool quantDiferente = false;

            string[] portas = SerialPort.GetPortNames();

            if (comboBox1.Items.Count == portas.Length)
            {
                foreach (string s in portas)
                {
                    if (!comboBox1.Items[i++].Equals(s))
                    {
                        quantDiferente = true;
                        break;
                    }
                }
            }
            else
            {
                quantDiferente = true;
            }

            if (!quantDiferente) return;

            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(portas);

            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Pode ser deixado vazio se não for necessário.
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            atualizaListaCOMs();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.PortName = comboBox1.SelectedItem.ToString();
                    serialPort1.Open();
                }
                catch
                {
                    return;
                }

                if (serialPort1.IsOpen)
                {
                    button1.Text = "Desconectar";
                    comboBox1.Enabled = false;
                    pictureBox1.Image = Image.FromFile("c:\\imagens\\Verde.bmp");
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    label1.Text = "Conectado";
                }
            }
            else
            {
                try
                {
                    serialPort1.Close();
                    comboBox1.Enabled = true;
                    button1.Text = "Conectar";
                    pictureBox1.Image = Image.FromFile("c:\\imagens\\Vermelho.bmp");
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    label1.Text = "Desconectado";
                }
                catch
                {
                    return;
                }
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // Le os dados recebidos e adiciona ao buffer
                bufferRecebido.Append(serialPort1.ReadExisting());

                // Se o dado contém uma quebra de linha, processa a infornaçao
                if (bufferRecebido.ToString().Contains("\n"))
                {
                    this.Invoke(new EventHandler(trataDadoRecebido));
                }

                    
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao receber dados: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile("c:\\imagens\\Vermelho.bmp");
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            pictureBox2.Image = Image.FromFile("c:\\imagens\\Vermelho.bmp");
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;

            serialPort1.DataReceived += serialPort1_DataReceived;

            label1.Text = "Desconectado";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                estadoLED = !estadoLED;

                serialPort1.WriteLine(estadoLED ? "L" : "D");

                pictureBox2.Image = Image.FromFile(estadoLED ? "c:\\imagens\\Verde.bmp" : "c:\\imagens\\Vermelho.bmp");
                button2.Text = estadoLED ? "Desligar" : "Ligar";
            }
        }

        private void lerDadosArduino(object sender, EventArgs e)
{
            try
            {
                string[] linhas = bufferRecebido.ToString().Split('\n');
                bufferRecebido.Clear();

                if (linhas.Length > 0)
                {
                    bufferRecebido.Append(linhas[linhas.Length - 1]);
                }

                // Agora dividimos os dados usando a vírgula como delimitador
                string dadosRecebidos = linhas[0].Trim();
                string[] dados = dadosRecebidos.Split(',');



                if (dados.Length == 3)
                {
                    double temperatura = double.Parse(dados[0].Replace(".", ","));
                    double tensaoA0 = double.Parse(dados[2].Replace(".", ","));

                    aGauge1.Value = Convert.ToInt32(temperatura); // Atualizando o valor do gauge da temperatura
                    aGauge3.Value = Convert.ToInt32(tensaoA0 * 10);

                    // Atualiza os rótulos na interface
                }
                else
                {
                    MessageBox.Show("Dados recebidos estão com formato incorreto.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao processar dados: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }

    }
}
