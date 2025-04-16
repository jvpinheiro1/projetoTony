using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace projeto1
{
    public partial class Form1 : Form
    {
        string RxString;

        public Form1()
        {
            InitializeComponent();
        }

        private void atualizaListaCOMs()
        {
            int i;
            bool quantDiferente; //flag para sinalizar que a quantixade de portas mudou

            i = 0;
            quantDiferente = false;

            //se a quantidade de portas mudeou
            if (comboBox1.Items.Count == SerialPort.GetPortNames().Length)
            {
                foreach (string s in SerialPort.GetPortNames())
                {
                    if (comboBox1.Items[i++].Equals(s) == false)
                    {
                        quantDiferente = true;
                    }
                }
            }
            else
            {
                quantDiferente = true;
            }

            //se não foi detectado diferença
            if (quantDiferente == false)
            {
                return;   //retorna
            }

            // limpa comboBox
            comboBox1.Items.Clear();

            //adciona todas as COM disponiveis na lista
            foreach (string s in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(s);
            }

            //seleciona a primeira posição da lista
            comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            atualizaListaCOMs();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == false)
            {
                try
                {
                    serialPort1.PortName = comboBox1.Items[comboBox1.SelectedIndex].ToString();
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

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile("c:\\imagens\\Vermelho.bmp");
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            label1.Text = "Desconectado";
        }
    }
}
