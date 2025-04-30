using System;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;

namespace projeto1
{
    public partial class Form1 : Form
    {
        private bool estadoLED = false;
        private string bufferRecebido = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile("c:\\imagens\\Vermelho.bmp");
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            pictureBox2.Image = Image.FromFile("c:\\imagens\\Vermelho.bmp");
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;

            pictureBox3.Image = Image.FromFile("c:\\imagens\\iconeArduino.png");
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;

            serialPort1.DataReceived += serialPort1_DataReceived;

            label1.Text = "Desconectado";
        }

        private void atualizaListaCOMs()
        {
            string[] portas = SerialPort.GetPortNames();

            if (!portas.SequenceEqual(comboBox1.Items.Cast<string>()))
            {
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(portas);
                if (comboBox1.Items.Count > 0)
                    comboBox1.SelectedIndex = 0;
            }
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

                    button1.Text = "Desconectar";
                    comboBox1.Enabled = false;
                    pictureBox1.Image = Image.FromFile("c:\\imagens\\Verde.bmp");
                    label1.Text = "Conectado";
                }
                catch
                {
                    MessageBox.Show("Erro ao conectar.");
                }
            }
            else
            {
                try
                {
                    serialPort1.DataReceived -= serialPort1_DataReceived;
                    serialPort1.Close();

                    button1.Text = "Conectar";
                    comboBox1.Enabled = true;
                    pictureBox1.Image = Image.FromFile("c:\\imagens\\Vermelho.bmp");
                    label1.Text = "Desconectado";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao desconectar: " + ex.Message);
                }
            }
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

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!serialPort1.IsOpen) return;

            try
            {
                string dados = serialPort1.ReadExisting();
                bufferRecebido += dados;

                if (bufferRecebido.Contains("\n"))
                {
                    this.Invoke(new Action(() =>
                    {
                        string[] linhas = bufferRecebido.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                        if (linhas.Length > 0)
                        {
                            string ultimaLinha = linhas[linhas.Length - 1];
                            bufferRecebido = ""; // limpar buffer após leitura

                            string[] partes = ultimaLinha.Split(',');

                            if (partes.Length == 2 &&
                                double.TryParse(partes[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double temperatura) &&
                                double.TryParse(partes[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double tensao))
                            {
                                thermControl1.UpdateControl((int)temperatura);
                                aGauge1.Value = (int)(tensao * 20); 
                            }
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("Erro na leitura da porta serial: " + ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
