using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Zadanie_domowe_1
{
	public partial class Form1 : Form
	{
		
		private Random random = new Random();
		public Form1()
		{
			InitializeComponent();
			this.comboBox_precision.SelectedIndex = 2;
		}

		private void button_Start(object sender, EventArgs e)
		{
			 double najM = double.MaxValue;
			double najW = double.MinValue;
			int a = 0, b = 0, n = 0, type = 0;
			int MinCzyMaks = 0; //1 to Minimum a 2 to Maksimum dla funckji g(x)
			double d = 0, pk = 0, pm = 0;
			int.TryParse(textBoxA.Text, out a);
			int.TryParse(textBoxB.Text, out b);
			int.TryParse(textBoxN.Text, out n);
			double.TryParse(textBoxPk.Text.Replace(".", ","), out pk);
			double.TryParse(textBoxPm.Text.Replace(".", ","), out pm);
			double.TryParse(comboBox_precision.SelectedItem.ToString(), out d);
			int L = l(a, b, d);

			switch (d)
			{
				case 0.1:
					type = 1;
					break;
				case 0.01:
					type = 2;
					break;
				case 0.001:
					type = 3;
					break;
				case 0.0001:
					type = 4;
					break;
			}

			if (radioButtonMinimum.Checked)
			{
				MinCzyMaks = 1;
			}
			else if (radioButtonMaksimum.Checked)
			{
				MinCzyMaks = 2;
			}


			double[] Xreal = new double[n];
			double[] f1x = new double[n];
			double[] g1x = new double[n];
			double[] p1x = new double[n];
			double[] q1x = new double[n];
			double[] r = new double[n];
			double[] xrps = new double[n];
			string[] xbinps = new string[n];
			string[] rodzice = new string[n];
			int[] PC = new int[n];
			string[] pokrzyzowanie = new string[n];
			string[] pmutacji = new string[n];
			string[] mutacje = new string[n];
			double[] binreal = new double[n];
			double[] realfx = new double[n];
			int ii = 0;
			while (ii < n)
			{
				double xr = xR(a, b, type);
				double f1 = fx(xr);

				Xreal[ii] = xr;
				f1x[ii] = f1;
				
				if (f1 < najM)
				{
					najM = f1;
				}
				
				
				if (f1 > najW)
				{
					najW = f1;
				}
				
				ii++;
			}

			//Liczenie gxa
			for (int j = 0; j < n; j++)
			{
				if (MinCzyMaks == 1)
				{
					g1x[j] = (double)(-1 * (f1x[j] - najW)) + getD(type);
				}
				else if (MinCzyMaks == 2)
				{
					g1x[j] = (double)(f1x[j] - najM) + getD(type);
				}

			}
			//liczenie pi
			double sumgx = 0;
			for (int j = 0; j < n; j++)
			{
				sumgx += g1x[j];
			}
			for (int j = 0; j < n; j++)
			{
				p1x[j] = g1x[j] / sumgx;
			}
			//liczenie qi
			double sumpi = 0;
			for (int j = 0; j < n; j++)
			{
				sumpi += p1x[j];
				q1x[j] = sumpi;
			}
			//losowanie liczby w zakresie 0 do 1 z precyzją
			for (int j = 0; j < n; j++)
			{
				r[j] = liczbaLosowa(type);
			}


			//losowanie xreala po selekcji
			for (int j = 0; j < n; j++)
			{
				if (0 < r[j] && r[j] <= q1x[0])
				{
					xrps[j] = Xreal[0];
				}
				else if (g1x[0] < r[j] && r[j] <= q1x[1])
				{
					xrps[j] = Xreal[1];
				}
				else
				{
					for (int iii = 1; iii < n; iii++)
					{
						if (q1x[iii - 1] < r[j] && r[j] <= q1x[iii])
						{
							xrps[j] = Xreal[iii];
							break;
						}
					}
				}
			}
			//liczenie xbina z xreal po selekcji
			for (int j = 0; j < n; j++)
			{
				int xint = xRI(a, b, L, xrps[j]);
				xbinps[j] = xIB(xint, L);
			}

			//rodzice
			for (int j = 0; j < n; j++)
			{
				if (liczbaLosowa(type) <= pk)
				{
					rodzice[j] = xbinps[j];
				}
				else
				{
					rodzice[j] = "Nie jest rodzicem";
				}
			}

			//PC
			for (int j = 0; j < n; j++)
			{
				if (rodzice[j] != "Nie jest rodzicem")
				{
					int llpc = liczbaLosowaciecie(L, type);
					PC[j] = llpc;
					for (int jj = j + 1; jj < n; jj++)
					{
						if (rodzice[jj] != "Nie jest rodzicem")
						{
							PC[jj] = llpc;
							j = jj;
							break;
						}
						else if (rodzice[jj] == "Nie jest rodzicem")
						{
							PC[jj] = -1;
						}
					}
				}
				else if (rodzice[j] == "Nie jest rodzicem")
				{
					PC[j] = -1;
				}
			}

			//pokrzyżowaniu + parzysta czy nie parzysta
			int count = 0;
			int indost = 0;
			for (int j = 0; j < n; j++)
			{
				if (rodzice[j] != "Nie jest rodzicem")
				{
					count++;
					indost = j;
				}
			}

			//pokrzyżowanie
			string upn1 = string.Empty;
			string utn1 = string.Empty;
			string upn2 = string.Empty;
			string utn2 = string.Empty;
			if (count % 2 == 0)
			{
				for (int j = 0; j < n; j++)
				{
					if (PC[j] != -1)
					{
						upn1 = rodzice[j].Substring(0, PC[j]);
						utn1 = rodzice[j].Substring(PC[j]);

						for (int jj = j + 1; jj < n; jj++)
						{
							if (PC[jj] == PC[j])
							{
								upn2 = rodzice[jj].Substring(0, PC[jj]);
								utn2 = rodzice[jj].Substring(PC[jj]);
								pokrzyzowanie[j] = upn1 + utn2;
								pokrzyzowanie[jj] = upn2 + utn1;
								j = jj;
								break;
							}
						}

					}

				}
			}
			else if (count % 2 == 1)
			{
				for (int j = 0; j < n; j++)
				{
					if (PC[j] != -1)
					{
						upn1 = rodzice[j].Substring(0, PC[j]);
						utn1 = rodzice[j].Substring(PC[j]);

						for (int jj = j + 1; jj < n; jj++)
						{
							if (PC[jj] == PC[j])
							{
								upn2 = rodzice[jj].Substring(0, PC[jj]);
								utn2 = rodzice[jj].Substring(PC[jj]);
								pokrzyzowanie[j] = upn1 + utn2;
								pokrzyzowanie[jj] = upn2 + utn1;
								j = jj;
								break;
							}
						}


						upn1 = rodzice[indost].Substring(0, PC[indost]);
						for (int jjj = 0; jjj < n; jjj++)
						{
							if (rodzice[jjj] != "Nie jest rodzicem")
							{
								utn2 = rodzice[jjj].Substring(PC[indost]);
								pokrzyzowanie[indost] = upn1 + utn2;
								break;
							}

						}
					}

				}
			}
			//
			for (int jj = 0; jj < n; jj++)
			{
				if (pokrzyzowanie[jj] == null)
				{
					pokrzyzowanie[jj] = xbinps[jj];
				}
			}
			//punkty mutacji
			for (int jj = 0; jj < n; jj++)
			{
				char[] mutacjatab = pokrzyzowanie[jj].ToCharArray();
				List<string> mutowanebity = new List<string>();

				for (int jjj = 0; jjj < pokrzyzowanie[jj].Length; jjj++)
				{
					double liczba = liczbaLosowar(type);

					if (liczba <= pm)
					{
						mutacjatab[jjj] = (mutacjatab[jjj] == '1') ? '0' : '1';
						mutowanebity.Add((jjj+1).ToString());
					}
				}
				pmutacji[jj] = string.Join(",", mutowanebity);
				mutacje[jj] = new string(mutacjatab);
			}

			//z bina do reala
			for (int jj = 0; jj < n; jj++) 
			{
				int liczbaint = xBI(mutacje[jj]);
				double liczbareal = xIR(liczbaint, a, b, type, L);
				binreal[jj] = liczbareal;
			}
			//f1x
			for (int jj = 0; jj < n; jj++)
			{
				realfx[jj] = fx(binreal[jj]);
			}




			int i = 0;
			dataGridView1.Rows.Clear(); // Usuń poprzednie wyniki przed nowymi obliczeniami
			while (i < n)
			{
				double xr = Xreal[i];
				double f1 = f1x[i];
				double gxV = g1x[i];
				double piV = p1x[i];
				double qiV = q1x[i];
				double rv = r[i];
				double xrpsv = xrps[i];
				string xbinpsv = xbinps[i];
				string rodzicev = rodzice[i];
				int PCv = PC[i];
				string pokrzyzowaniev = pokrzyzowanie[i];
				string pmutacjiv = pmutacji[i];
				string mutacjev = mutacje[i];
				double binrealv = binreal[i];
				double f1xv = f1x[i];
				
				i++;

				dataGridView1.Rows.Add(i, xr, f1, gxV, piV, qiV, rv, xrpsv, xbinpsv, rodzicev, PCv, pokrzyzowaniev, pmutacjiv, mutacjev, binrealv, f1xv);
			}
		}

		// Losowanie liczby w zakresie od a do b
		double xR(int a, int b, int type)
		{
			
			double xr = Math.Round(random.NextDouble() * (b - a) + a, type);
			return xr;
		}
		//losowanie liczby losowej w zakresie (0,1) z dokładnością d
		double liczbaLosowa(int type)
		{
			
			double losli = Math.Round(random.NextDouble() * (1 - getD(type)) + getD(type), type);
			return losli;
		}
		double liczbaLosowar(int type)
		{
			
			double losr = Math.Round(random.NextDouble() * 1 - double.Epsilon, type);
			return losr;
		}

		//LIczba losowa dla punktu cięcia
		//llpc - liczba losowa - punkt ciecia
		int liczbaLosowaciecie(int L, int type)
		{
			Random random = new Random();
			int llpc = random.Next(1, L - 1);
			return llpc;
		}

		// Liczba L, jaka potęga dwójki aby zmieścić wszystkie opcje z daną precyzją
		int l(int a, int b, double d)
		{
			int L = (int)Math.Ceiling(Math.Log2((b - a) / d) + 1);
			return L;
		}

		// xReal -> xInt
		int xRI(int a, int b, int L, double xrps)
		{
			int xRTxI = (int)Math.Round((xrps - a) * (Math.Pow(2, L) - 1) / (b - a));
			return xRTxI;
		}

		// xInt --> xReal
		double xIR(int xBTxI, int a, int b, int type, int L)
		{
			double xITxR = Math.Round(a + (xBTxI * (b - a)) / (Math.Pow(2, L) - 1), type);
			return xITxR;
		}

		// xInt --> xBin
		string xIB(int xRTxI, int L)
		{
			string xITxB = Convert.ToString(xRTxI, 2).PadLeft(L, '0');
			return xITxB;
		}

		// xBin --> xInt
		int xBI(string xITxB)
		{
			int xBTxI = Convert.ToInt32(xITxB, 2);
			return xBTxI;
		}
		//funckja f(x)
		double fx(double xr)
		{
			double fx = (xr % 1) * (Math.Cos(20 * Math.PI * xr) - Math.Sin(xr));
			return fx;
		}

		double getD(int decimalPlaces)
		{
			switch (decimalPlaces)
			{
				case 1:
					return 0.1;

				case 2:
					return 0.01;

				case 3:
					return 0.001;

				case 4:
					return 0.0001;
				default:
					return 0;
			}
		}

	}
}