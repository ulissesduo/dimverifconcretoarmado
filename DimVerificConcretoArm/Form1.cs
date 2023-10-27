using System.Globalization;

namespace DimVerificConcretoArm
{
    public partial class Form1 : Form
    {

        //Declarando variáveis públicas
        public double esl, fyd, tsl;
        public double aas, asl, es;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double fck, fyk, gamac, gamas, gamaf, bduct;
            double b, d, h, dl, delta, amk;
            double alamb, alfac, eu, qlim, ami, amilim;
            double fcd, tcd, amd;
            double qsi, qsia, romin, asmin, a;
            //
            //
            //Esta subrotina corresponde ao botão CALCULAR.
            //
            //
            //Dimensionamento de seções retangulares à flexão normal simples
            //
            //ENTRADA DE DADOS
            //
            //Os dados são lidos das caixas de texto do formulário
            //
            //Resistência característica à compressão do concreto em MPa
            //Substituindo a vírgula por ponto
            textBox1.Text = textBox1.Text.Replace(",", ".");
            fck = Convert.ToDouble(textBox1.Text, CultureInfo.InvariantCulture);
            //
            //Tensão de escoamento característica do aço em MPa
            textBox2.Text = textBox2.Text.Replace(",", ".");
            fyk = Convert.ToDouble(textBox2.Text, CultureInfo.InvariantCulture);
            //
            //Módulo de elasticidade do aço em GPa
            textBox3.Text = textBox3.Text.Replace(",", ".");
            es = Convert.ToDouble(textBox3.Text, CultureInfo.InvariantCulture);
            //
            //Coeficientes parciais de segurança:
            //para o concreto
            textBox6.Text = textBox6.Text.Replace(",", ".");
            gamac = Convert.ToDouble(textBox6.Text, CultureInfo.InvariantCulture);
            //
            //para o aço
            textBox5.Text = textBox5.Text.Replace(",", ".");
            gamas = Convert.ToDouble(textBox5.Text, CultureInfo.InvariantCulture);
            //
            //para o momento fletor
            textBox4.Text = textBox4.Text.Replace(",", ".");
            gamaf = Convert.ToDouble(textBox4.Text, CultureInfo.InvariantCulture);
            //
            //Coeficiente beta de redistribuição de momentos
            textBox7.Text = textBox7.Text.Replace(",", ".");
            bduct = Convert.ToDouble(textBox7.Text, CultureInfo.InvariantCulture);
            //
            //Largura da seção transversal
            textBox8.Text = textBox8.Text.Replace(",", ".");
            b = Convert.ToDouble(textBox8.Text, CultureInfo.InvariantCulture);
            //
            //Altura da seção transversal
            textBox9.Text = textBox9.Text.Replace(",", ".");
            h = Convert.ToDouble(textBox9.Text, CultureInfo.InvariantCulture);
            //
            //Altura útil
            textBox11.Text = textBox11.Text.Replace(",", ".");
            d = Convert.ToDouble(textBox11.Text, CultureInfo.InvariantCulture);
            //
            //Parâmetro d'
            textBox10.Text = textBox10.Text.Replace(",", ".");
            dl = Convert.ToDouble(textBox10.Text, CultureInfo.InvariantCulture);
            //
            //Momento fletor de serviço em kNm
            textBox12.Text = textBox12.Text.Replace(",", ".");

            amk = Convert.ToDouble(textBox12.Text, CultureInfo.InvariantCulture);
            //
            //
            //FIM DA ENTRADA DE DADOS
            //
            //INÍCIO DOS CÁLCULOS
            //
            //
            //Parâmetros do diagrama retangular
            if (fck <= 50)
            {
                alamb = 0.8;
                alfac = 0.85;
                eu = 3.5;
                qlim = 0.8 * bduct - 0.35;
            }
            else
            {
                alamb = 0.8 - (fck - 50) / 400;
                alfac = 0.85 * (1 - (fck - 50) / 200);
                a = (90 - fck) / 100;
                eu = 2.6 + 35 * Math.Pow(a, 4);
                qlim = 0.8 * bduct - 0.45;
            }
            //
            //Conversão de unidades: transformando para kN e cm
            amk = 100 * amk;
            fck = fck / 10;
            fyk = fyk / 10;
            es = 100 * es;
            //
            //Resistências de cálculo
            fcd = fck / gamac;
            tcd = alfac * fcd;
            fyd = fyk / gamas;
            amd = gamaf * amk;
            //
            //Parâmetro geométrico
            delta = dl / d;
            //
            //Momento limite
            amilim = alamb * qlim * (1 - 0.5 * alamb * qlim);
            //
            //Momento reduzido solicitante
            ami = amd / (b * d * d * tcd);
            //
            if (ami <= amilim)
            {
                //Armadura simples
                qsi = (1 - Math.Sqrt(1 - 2 * ami)) / alamb;
                aas = alamb * qsi * b * d * tcd / fyd;
                asl = 0;
            }
            if (ami > amilim)
            {
                //Armadura dupla
                //
                //Evitando armadura dupla no domínio 2
                qsia = eu / (eu + 10);
                if (qlim < qsia)
                {
                    //
                    // Está resultando armadura dupla no domínio 2.
                    // Colocar mensagem para o usuário aumentar as dimensões da seção transversal e parar o processamento
                    //
                    MessageBox.Show("Resultou armadura dupla no domínio 2. Aumente as dimensões da seção transversal");
                    return;
                }
                //
                // Eliminando o caso em que qlim<delta
                // Se isto ocorrer, a armadura de compressão estará tracionada
                //
                if (qlim <= delta)
                {
                    //
                    // Colocar mensagem para o usuário aumentar as dimensões da seção transversal e parar oprocessamento
                    //
                    MessageBox.Show("Aumente as dimensões da seção transversal");
                    return;
                }
                //
                //Deformação da armadura de compressão
                esl = eu * (qlim - delta) / qlim;
                esl = esl / 1000;
                // Tensão na armadura de compressão
                // Chamar Sub-rotina
                Tensao();
                asl = (ami - amilim) * b * d * tcd / ((1 - delta) * tsl);
                aas = (alamb * qlim + (ami - amilim) / (1 - delta)) * b * d * tcd / fyd;
            }
            //
            //Armadura mínima
            a = 2.0 / 3.0;
            fck = 10 * fck;
            fyd = 10 * fyd;
            if (fck <= 50)
            {
                romin = 0.078 * Math.Pow(fck, a) / fyd;
            }
            else
            {
                romin = 0.5512 * Math.Log(1 + 0.11 * fck) / fyd;
            }
            if (romin < 0.0015)
            {
                romin = 0.0015;
            }
            //
            asmin = romin * b * h;
            if (aas < asmin)
            {
                aas = asmin;
            }
            //
            //Convertendo a saída para duas casas decimais
            //
            decimal saida1 = Decimal.Round(Convert.ToDecimal(aas), 2);
            decimal saida2 = Decimal.Round(Convert.ToDecimal(asl), 2);
            //
            //MOSTRAR O RESULTADO
            //Área da armadura tracionada: aas
            //Área da armadura comprimida: asl
            //
            textBox14.Text = Convert.ToString(saida1);
            textBox13.Text = Convert.ToString(saida2);
        }

        private void Tensao()
        {
            double ess, eyd;
            //
            //Calcula a tensão no aço
            //es = módulo de elasticidade do aço em kN/cm2
            //esl = deformação de entrada
            //fyd = tensão de escoamento de cálculo em kN/cm2
            //tsl = tensão de saída em kN/cm2
            //
            //Trabalhando com deformação positiva
            ess = Math.Abs(esl);
            eyd = fyd / es;
            if (ess < eyd)
            {
                tsl = es * ess;
            }
            else
            {
                tsl = fyd;
            }
            //Trocando o sinal se necessário
            if (esl < 0)
            {
                tsl = -tsl;
            }
        }

        //2.6 – Exemplos para testar os programas
    }
}