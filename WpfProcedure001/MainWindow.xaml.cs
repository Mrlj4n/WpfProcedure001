using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;

namespace WpfProcedure001
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //GEN LISTA VRACA KOR

        private List<Korisnik> VratiKorisnike()
        {
            List<Korisnik> listaKorisnika = new List<Korisnik>();
            using (SqlConnection konekcija = new SqlConnection(Konekcija.cnnKorisnikDb))
            {
                using (SqlCommand komanda = new SqlCommand("PrikaziKorisnike", konekcija))
                {
                    komanda.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        konekcija.Open();                        
                        using (SqlDataReader dr = komanda.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                Korisnik k = new Korisnik
                                {
                                    KorisnikId = dr.GetInt32(0),
                                    Ime = dr.GetString(1),
                                    Prezime = dr.GetString(2),
                                    Email = dr.GetString(3)
                                };

                                listaKorisnika.Add(k);
                            }
                        }
                        return listaKorisnika;
                    }
                    catch (Exception)
                    {
                        return null;
                    }

                }
            }
        }

        
        private void PrikaziKorisnike()
        {
            ComboBox1.Items.Clear();
            List<Korisnik> listaKorisnika = VratiKorisnike();

            if (listaKorisnika != null)
            {
                foreach (Korisnik k in listaKorisnika)
                {
                    ComboBox1.Items.Add(k);
                }
            }
            
        }


        //RESETUJ GUI
        private void Resetuj()
        {
            TextBoxId.Clear();
            TextBoxIme.Clear();
            TextBoxPrezime.Clear();
            TextBoxEmail.Clear();
            ComboBox1.SelectedIndex =- 1;
        }

        //Pom metoda 
        private Korisnik NadjiStavku(int id)
        {
            foreach (Korisnik k in ComboBox1.Items)
            {
                if (k.KorisnikId == id)
                {
                    return k;
                }
            }

            return null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PrikaziKorisnike();
        }

        private void ButtonResetuj_Click(object sender, RoutedEventArgs e)
        {
            Resetuj();
        }

        private void ButtonUbaci_Click(object sender, RoutedEventArgs e)
        {
            int ID = 0;
            string poruka = "";
            using (SqlConnection konekcija = new SqlConnection(Konekcija.cnnKorisnikDb))
            {
                using (SqlCommand komanda = new SqlCommand("UbaciKorisnika", konekcija))
                {
                    komanda.CommandType = CommandType.StoredProcedure;
                    SqlParameter KorisnikId = new SqlParameter("@KorisnikId", SqlDbType.Int);
                    KorisnikId.Direction = ParameterDirection.Output;
                    komanda.Parameters.Add(KorisnikId);

                    try
                    {
                        komanda.Parameters.AddWithValue("@Ime", TextBoxIme.Text);
                        komanda.Parameters.AddWithValue("@Prezime", TextBoxPrezime.Text);
                        komanda.Parameters.AddWithValue("@Email", TextBoxEmail.Text);

                        konekcija.Open();

                        komanda.ExecuteNonQuery();
                        ID = (int)KorisnikId.Value;
                        
                    }
                    catch (Exception xcp)
                    {

                        poruka = xcp.Message;
                    }

                }
            }

            if (!string.IsNullOrWhiteSpace(poruka))
            {
                MessageBox.Show(poruka);
            }
            else
            {
                PrikaziKorisnike();
                ComboBox1.SelectedItem = NadjiStavku(ID);
                MessageBox.Show("Korisnik ubacen");
            }

        }

        private void ButtonPromeni_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBox1.SelectedIndex > -1)
            {
                Korisnik k = ComboBox1.SelectedItem as Korisnik;
                string poruka = "";
                using (SqlConnection konekcija = new SqlConnection(Konekcija.cnnKorisnikDb))
                {
                    using (SqlCommand komanda = new SqlCommand("PromeniKorisnika", konekcija))
                    {
                        komanda.CommandType = CommandType.StoredProcedure;

                        try
                        {
                            komanda.Parameters.AddWithValue("@KorisnikId", TextBoxId.Text);
                            komanda.Parameters.AddWithValue("@Ime", TextBoxIme.Text);
                            komanda.Parameters.AddWithValue("@Prezime", TextBoxPrezime.Text);
                            komanda.Parameters.AddWithValue("@Email", TextBoxEmail.Text);

                            konekcija.Open();
                            komanda.ExecuteNonQuery();
                        }
                        catch (Exception xcp)
                        {

                            poruka = xcp.Message;
                        }

                    }
                }

                if (!string.IsNullOrWhiteSpace(poruka))
                {
                    MessageBox.Show(poruka);
                }
                else
                {
                    PrikaziKorisnike();
                    ComboBox1.SelectedItem = NadjiStavku(k.KorisnikId);
                    MessageBox.Show("Korisnik promenjen");
                }
            }
            else
            {
                MessageBox.Show("Odaberite korisnika");
                return;
            }
        }

        private void ButtonObrisi_Click(object sender, RoutedEventArgs e)
        {
            if (ComboBox1.SelectedIndex > -1)
            {
                string poruka = "";
                using (SqlConnection konekcija = new SqlConnection(Konekcija.cnnKorisnikDb))
                {
                    using (SqlCommand komanda = new SqlCommand("ObrisiKorisnika", konekcija))
                    {
                        komanda.CommandType = CommandType.StoredProcedure;

                        try
                        {
                            komanda.Parameters.AddWithValue("@KorisnikId", TextBoxId.Text);
                            konekcija.Open();
                            komanda.ExecuteNonQuery();
                        }
                        catch (Exception xcp)
                        {

                            poruka = xcp.Message;
                        }

                    }
                }

                if (!string.IsNullOrWhiteSpace(poruka))
                {
                    MessageBox.Show(poruka);
                }
                else
                {
                    PrikaziKorisnike();
                    Resetuj();
                    MessageBox.Show("Korisnik obrisan");
                }
            }
            else
            {
                MessageBox.Show("Odaberite korisnika");
            }
        }

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox1.SelectedIndex > -1)
            {
                Korisnik k = ComboBox1.SelectedItem as Korisnik;
                TextBoxId.Text = k.KorisnikId.ToString();
                TextBoxIme.Text = k.Ime;
                TextBoxPrezime.Text = k.Prezime;
                TextBoxEmail.Text = k.Email;

            }
        }
    }
}
