using System.Globalization;

namespace DimVerificConcretoArm
{
    public partial class Form1 : Form
    {

        //Declarando vari�veis p�blicas
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
            //Esta subrotina corresponde ao bot�o CALCULAR.
            //
            //
            //Dimensionamento de se��es retangulares � flex�o normal simples
            //
            //ENTRADA DE DADOS
            //
            //Os dados s�o lidos das caixas de texto do formul�rio
            //
            //Resist�ncia caracter�stica � compress�o do concreto em MPa
            //Substituindo a v�rgula por ponto
            textBox1.Text = textBox1.Text.Replace(",", ".");
            fck = Convert.ToDouble(textBox1.Text, CultureInfo.InvariantCulture);
            //
            //Tens�o de escoamento caracter�stica do a�o em MPa
            textBox2.Text = textBox2.Text.Replace(",", ".");
            fyk = Convert.ToDouble(textBox2.Text, CultureInfo.InvariantCulture);
            //
            //M�dulo de elasticidade do a�o em GPa
            textBox3.Text = textBox3.Text.Replace(",", ".");
            es = Convert.ToDouble(textBox3.Text, CultureInfo.InvariantCulture);
            //
            //Coeficientes parciais de seguran�a:
            //para o concreto
            textBox6.Text = textBox6.Text.Replace(",", ".");
            gamac = Convert.ToDouble(textBox6.Text, CultureInfo.InvariantCulture);
            //
            //para o a�o
            textBox5.Text = textBox5.Text.Replace(",", ".");
            gamas = Convert.ToDouble(textBox5.Text, CultureInfo.InvariantCulture);
            //
            //para o momento fletor
            textBox4.Text = textBox4.Text.Replace(",", ".");
            gamaf = Convert.ToDouble(textBox4.Text, CultureInfo.InvariantCulture);
            //
            //Coeficiente beta de redistribui��o de momentos
            textBox7.Text = textBox7.Text.Replace(",", ".");
            bduct = Convert.ToDouble(textBox7.Text, CultureInfo.InvariantCulture);
            //
            //Largura da se��o transversal
            textBox8.Text = textBox8.Text.Replace(",", ".");
            b = Convert.ToDouble(textBox8.Text, CultureInfo.InvariantCulture);
            //
            //Altura da se��o transversal
            textBox9.Text = textBox9.Text.Replace(",", ".");
            h = Convert.ToDouble(textBox9.Text, CultureInfo.InvariantCulture);
            //
            //Altura �til
            textBox11.Text = textBox11.Text.Replace(",", ".");
            d = Convert.ToDouble(textBox11.Text, CultureInfo.InvariantCulture);
            //
            //Par�metro d'
            textBox10.Text = textBox10.Text.Replace(",", ".");
            dl = Convert.ToDouble(textBox10.Text, CultureInfo.InvariantCulture);
            //
            //Momento fletor de servi�o em kNm
            textBox12.Text = textBox12.Text.Replace(",", ".");

            amk = Convert.ToDouble(textBox12.Text, CultureInfo.InvariantCulture);
            //
            //
            //FIM DA ENTRADA DE DADOS
            //
            //IN�CIO DOS C�LCULOS
            //
            //
            //Par�metros do diagrama retangular
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
            //Convers�o de unidades: transformando para kN e cm
            amk = 100 * amk;
            fck = fck / 10;
            fyk = fyk / 10;
            es = 100 * es;
            //
            //Resist�ncias de c�lculo
            fcd = fck / gamac;
            tcd = alfac * fcd;
            fyd = fyk / gamas;
            amd = gamaf * amk;
            //
            //Par�metro geom�trico
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
                //Evitando armadura dupla no dom�nio 2
                qsia = eu / (eu + 10);
                if (qlim < qsia)
                {
                    //
                    // Est� resultando armadura dupla no dom�nio 2.
                    // Colocar mensagem para o usu�rio aumentar as dimens�es da se��o transversal e parar o processamento
                    //
                    MessageBox.Show("Resultou armadura dupla no dom�nio 2. Aumente as dimens�es da se��o transversal");
                    return;
                }
                //
                // Eliminando o caso em que qlim<delta
                // Se isto ocorrer, a armadura de compress�o estar� tracionada
                //
                if (qlim <= delta)
                {
                    //
                    // Colocar mensagem para o usu�rio aumentar as dimens�es da se��o transversal e parar oprocessamento
                    //
                    MessageBox.Show("Aumente as dimens�es da se��o transversal");
                    return;
                }
                //
                //Deforma��o da armadura de compress�o
                esl = eu * (qlim - delta) / qlim;
                esl = esl / 1000;
                // Tens�o na armadura de compress�o
                // Chamar Sub-rotina
                Tensao();
                asl = (ami - amilim) * b * d * tcd / ((1 - delta) * tsl);
                aas = (alamb * qlim + (ami - amilim) / (1 - delta)) * b * d * tcd / fyd;
            }
            //
            //Armadura m�nima
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
            //Convertendo a sa�da para duas casas decimais
            //
            decimal saida1 = Decimal.Round(Convert.ToDecimal(aas), 2);
            decimal saida2 = Decimal.Round(Convert.ToDecimal(asl), 2);
            //
            //MOSTRAR O RESULTADO
            //�rea da armadura tracionada: aas
            //�rea da armadura comprimida: asl
            //
            textBox14.Text = Convert.ToString(saida1);
            textBox13.Text = Convert.ToString(saida2);
        }

        private void Tensao()
        {
            double ess, eyd;
            //
            //Calcula a tens�o no a�o
            //es = m�dulo de elasticidade do a�o em kN/cm2
            //esl = deforma��o de entrada
            //fyd = tens�o de escoamento de c�lculo em kN/cm2
            //tsl = tens�o de sa�da em kN/cm2
            //
            //Trabalhando com deforma��o positiva
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
            //Trocando o sinal se necess�rio
            if (esl < 0)
            {
                tsl = -tsl;
            }
        }

        //2.6 � Exemplos para testar os programas
    }
}