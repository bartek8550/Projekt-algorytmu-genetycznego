using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

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
			int a = 0, b = 0, n = 0, type = 0, T = 0;
			bool iselite = false;
			int MinCzyMaks = 0; //1 to Minimum a 2 to Maksimum dla funckji g(x)
			double d = 0, pk = 0, pm = 0;
			int.TryParse(textBoxA.Text, out a);
			int.TryParse(textBoxB.Text, out b);
			int.TryParse(textBoxN.Text, out n);
			int.TryParse(textBoxT.Text, out T);
			iselite = checkBoxElite.Checked;
			double.TryParse(textBoxPk.Text.Replace(".", ","), out pk);
			double.TryParse(textBoxPm.Text.Replace(".", ","), out pm);
			double.TryParse(comboBox_precision.SelectedItem.ToString(), out d);
			int L = l(a, b, d);

			List<double> allmin = new List<double>();
			List<double> allmax = new List<double>();
			List<double> allavg = new List<double>();

			Wykres.Series.Clear();

			var Obszar = Wykres.ChartAreas[0];
			Obszar.AxisY.Minimum = -3;
			Obszar.AxisY.Maximum = 3;
			Obszar.AxisX.Minimum = 0;
			var fmin = new Series("Fmin") { ChartType = SeriesChartType.Line, Color = Color.YellowGreen };
			var fmax = new Series("Fmax") { ChartType = SeriesChartType.Line, Color = Color.OrangeRed }; ;
			var favg = new Series("Favg") { ChartType = SeriesChartType.Line, Color = Color.DarkMagenta };
			Wykres.Series.Add(fmin);
			Wykres.Series.Add(fmax);
			Wykres.Series.Add(favg);

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
			List<double> newxreals = new List<double>();




			for (int iiii = 0; iiii < T; iiii++)
			{
				int elindex = 0;
				double elxr = 0;
				double elfx = double.MinValue;


				int ii = 0;
				while (ii < n)
				{
					double xr = 0;
					if (newxreals.Count() > 0)
					{
						xr = newxreals[ii];
					}
					else
					{
						xr = xR(a, b, type);
					}
					double f1 = fx(xr);
					if (f1 > elfx)
					{
						elindex = ii;
						elxr = xr;
						elfx = f1;
					}

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
							mutowanebity.Add((jjj + 1).ToString());
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
				//jeśli z elitą
				if (iselite && realfx[elindex] < elfx)
				{
					binreal[elindex] = elxr;
					realfx[elindex] = elfx;
				}
				else if (iselite)
				{
					List<int> indtocheck = new List<int>();
					for (int jj = 0; jj < n; jj++)
					{
						if (jj != elindex)
						{
							indtocheck.Add(jj);
						}
					}
					List<int> shuffleindexes = indtocheck.OrderBy(i => Guid.NewGuid()).ToList();
					for (int jj = 0; jj < shuffleindexes.Count(); jj++)
					{
						if (realfx[shuffleindexes[jj]] < elfx)
						{
							binreal[shuffleindexes[jj]] = elxr;
							realfx[shuffleindexes[jj]] = elfx;
							break;
						}
					}
				}


				newxreals.Clear();
				//Na zajęcia 3
				for (int jj = 0; jj < n; jj++)
				{
					newxreals.Add(binreal[jj]);
				}

				double fmaxtejiteracji = realfx.Max();
				double fmintejiteracji = realfx.Min();
				double favgtejiteracji = realfx.Average();

				allmin.Add(fmintejiteracji);
				allmax.Add(fmaxtejiteracji);
				allavg.Add(favgtejiteracji);

			}

			for (int jkj = 0; jkj < T; jkj++)
			{
				fmin.Points.AddXY(jkj, allmin[jkj]);
				fmax.Points.AddXY(jkj, allmax[jkj]);
				favg.Points.AddXY(jkj, allavg[jkj]);
			}

			Dictionary<double, int> Xrealwystapieniaboczemunie = new Dictionary<double, int>();
			for (int jp = 0; jp < n; jp++)
			{
				if (Xrealwystapieniaboczemunie.ContainsKey(newxreals[jp]))
				{
					Xrealwystapieniaboczemunie[newxreals[jp]]++;
				}
				else
				{
					Xrealwystapieniaboczemunie[newxreals[jp]] = 1;
				}
			}
			double[] xrpodsumowanie = new double[Xrealwystapieniaboczemunie.Count()];
			string[] xbinpodsumowanie = new string[Xrealwystapieniaboczemunie.Count()];
			double[] fxpodsumowanie = new double[Xrealwystapieniaboczemunie.Count()];
			double[] procentpodsumowanie = new double[Xrealwystapieniaboczemunie.Count()];
			int iterator = 0;
			dataGridView2.Rows.Clear();
			foreach (KeyValuePair<double, int> keyValuePair in Xrealwystapieniaboczemunie)
			{
				int xrealtoint = xRI(a, b, L, keyValuePair.Key);
				string inttobin = xIB(xrealtoint, L);
				double fx2 = fx(keyValuePair.Key);
				double procent = (double)keyValuePair.Value / n * 100;

				xrpodsumowanie[iterator] = keyValuePair.Key;
				xbinpodsumowanie[iterator] = inttobin;
				fxpodsumowanie[iterator] = fx2;
				procentpodsumowanie[iterator] = procent;
				iterator++;

			}
			List<(double, string, double, double)> calepodsumowanie = xrpodsumowanie.Select((wartosc, index) => (wartosc, xbinpodsumowanie[index], fxpodsumowanie[index], procentpodsumowanie[index])).ToList();
			List<(double, string, double, double)> posortowanePodsumowanie = calepodsumowanie.OrderByDescending(item => item.Item4).ToList();
			for (int hjk = 0; hjk < posortowanePodsumowanie.Count(); hjk++)
			{
				dataGridView2.Rows.Add(hjk + 1, posortowanePodsumowanie[hjk].Item1, posortowanePodsumowanie[hjk].Item2, posortowanePodsumowanie[hjk].Item3, posortowanePodsumowanie[hjk].Item4);
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

			double losr = random.NextDouble();
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



		private async void button2_Click(object sender, EventArgs e)
		{
			int[] Nwartosci = { 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80 };
			int[] Twartosci = { 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150 };
			double[] PKwartosci = { 0.5, 0.55, 0.6, 0.65, 0.7, 0.75, 0.8, 0.85, 0.9 };
			double[] PMwartosci = { 0.0001, 0.0005, 0.001, 0.0015, 0.002, 0.0025, 0.003, 0.0035, 0.004, 0.0045, 0.005, 0.0055, 0.006, 0.0065, 0.007, 0.0075, 0.008, 0.0085, 0.009, 0.0095, 0.01 };
			int a = -4;
			int b = 12;
			double d = 0.001;
			int L = l(a, b, d);
			int type = 3;
			bool iselite = true;

			int powtorzenie = 1;
			List<(int, int, double, double, int, double)> wyniki = new List<(int, int, double, double, int, double)>();
			List<Task> tasks = new List<Task>();
			foreach (int n in Nwartosci)
			{
				foreach (int T in Twartosci)
				{
					foreach (double pk in PKwartosci)
					{
						foreach (double pm in PMwartosci)
						{
							tasks.Add(Task.Run(() =>
							{
								for (int i = 0; i < 100; i++)
								{
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
									List<double> newxreals = new List<double>();
									double najM = double.MaxValue;
									double najW = double.MinValue;
									int MinCzyMaks = 2;
									List<double> allmax = new List<double>();

									for (int iiii = 0; iiii < T; iiii++)
									{
										int elindex = 0;
										double elxr = 0;
										double elfx = double.MinValue;


										int ii = 0;
										while (ii < n)
										{
											double xr = 0;
											if (newxreals.Count() > 0)
											{
												xr = newxreals[ii];
											}
											else
											{
												xr = xR(a, b, type);
											}
											double f1 = fx(xr);
											if (f1 > elfx)
											{
												elindex = ii;
												elxr = xr;
												elfx = f1;
											}

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
													mutowanebity.Add((jjj + 1).ToString());
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

										if (iselite && realfx[elindex] < elfx)
										{
											binreal[elindex] = elxr;
											realfx[elindex] = elfx;
										}
										else if (iselite)
										{
											List<int> indtocheck = new List<int>();
											for (int jj = 0; jj < n; jj++)
											{
												if (jj != elindex)
												{
													indtocheck.Add(jj);
												}
											}
											List<int> shuffleindexes = indtocheck.OrderBy(i => Guid.NewGuid()).ToList();
											for (int jj = 0; jj < shuffleindexes.Count(); jj++)
											{
												if (realfx[shuffleindexes[jj]] < elfx)
												{
													binreal[shuffleindexes[jj]] = elxr;
													realfx[shuffleindexes[jj]] = elfx;
													break;
												}
											}
										}
										newxreals.Clear();

										for (int jj = 0; jj < n; jj++)
										{
											newxreals.Add(binreal[jj]);
										}
										double fmaxtejiteracji = realfx.Max();
										allmax.Add(fmaxtejiteracji);




									}
									lock (wyniki)
									{
										wyniki.Add((powtorzenie, n, pk, pm, T, allmax.Average()));
										powtorzenie++;
									}


								}


							}));

						}
					}
				}
			}
			await Task.WhenAll(tasks);

			List<(int, int, double, double, int, double)> posortowanewyniki = wyniki.OrderByDescending(item => item.Item6).ToList();
			int lp = 1;
			dataGridView3.Rows.Clear();
			for (int hj = 0; hj < 50; hj++)
			{
				dataGridView3.Rows.Add(lp, posortowanewyniki[hj].Item2, posortowanewyniki[hj].Item3, posortowanewyniki[hj].Item4, posortowanewyniki[hj].Item5, posortowanewyniki[hj].Item6);
				lp++;
			}


		}
	}
}