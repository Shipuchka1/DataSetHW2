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
using System.Data.Sql;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace DataSetHW2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public string ConStr = "Data Source = DESKTOP-DKRR6L1; Initial Catalog = DataSet2; User Id = Natalya;Password = 123";
        public static SqlConnection con;
        public MainWindow()
        {
            con = new SqlConnection(ConStr);


            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            Item_Click(sender);
        }

        private void GroupMenuItem_Click(object sender, RoutedEventArgs e)
        {
            GW.Columns.Clear();
            WrapRadio.Visibility = Visibility.Visible;
            
        }

        public void Item_Click(object sender)
        {
            try
            {
                WrapRadio.Visibility = Visibility.Hidden;
                GW.Columns.Clear();
                MenuItem mit = (MenuItem)sender;
                bool primFlag = false;
                bool ModelFlag = false;
                con.Open();
                DataSet ds = new DataSet(mit.Header.ToString());
                DataTable dt = new DataTable(mit.Header + "Table");
                SqlDataAdapter da = new SqlDataAdapter("select * from dic_" + mit.Header, con);
                SqlCommand cmd = new SqlCommand("exec sp_columns dic_" + mit.Header, con);
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.Default);
                int i = 0; string str = "";
                DataColumn prim = new DataColumn();
                DataTableMapping map = da.TableMappings.Add("dic_Model", "ModelTable");
                if (mit.Header.ToString() == "Model")
                {
                    map.ColumnMappings.Add("ModelId", "ModelId");
                    map.ColumnMappings.Add("CodeModel", "Code");
                    map.ColumnMappings.Add("NameModel", "Name");
                    map.ColumnMappings.Add("Series", "SeriesId");
                    ModelFlag = true;
                }

                GridViewColumn temp = new GridViewColumn();

                int c = 0;
                while (dr.Read())
                {
                    temp = new GridViewColumn();
                    if (dr[5].ToString().Contains("int") && !dr[5].ToString().Contains("identity"))
                    {
                        dt.Columns.Add(new DataColumn(dr[3].ToString(), i.GetType()));
                        if (ModelFlag)
                        {
                            temp.Header = map.ColumnMappings[c].ToString();
                            c++;
                        }
                        else
                            temp.Header = dr[3].ToString();
                        temp.DisplayMemberBinding = new Binding(dr[3].ToString());
                    }
                    if (dr[5].ToString().Contains("varchar"))
                    {
                        dt.Columns.Add(new DataColumn(dr[3].ToString(), str.GetType()));
                        if (ModelFlag)
                        {
                            temp.Header = map.ColumnMappings[c].ToString();
                            c++;
                        }
                        else
                            temp.Header = dr[3].ToString();
                        temp.DisplayMemberBinding = new Binding(dr[3].ToString());
                    }

                    if (dr[5].ToString().Contains("identity"))
                    {
                        primFlag = true;
                        prim = new DataColumn(dr[3].ToString(), typeof(int));
                        dt.Columns.Add(prim);
                        if (ModelFlag)
                        {
                            temp.Header = map.ColumnMappings[c].ToString();
                            c++;
                        }
                        else
                            temp.Header = dr[3].ToString();
                        temp.DisplayMemberBinding = new Binding(dr[3].ToString());
                    }
                    GW.Columns.Add(temp);

                }

                if (primFlag)
                {
                    dt.PrimaryKey = new DataColumn[]
               {
                prim
               };
                    prim.AutoIncrement = true;
                    prim.AutoIncrementSeed = 0;
                    prim.AutoIncrementStep = 1;
                }

                con.Close();
                con.Open();

                da.Fill(dt);
                ds.Tables.Add(dt);

                DataListView.ItemsSource = ds.Tables[0].DefaultView;

                con.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadDataButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                GW.Columns.Clear();
                con.Open();
                List<Group> lg = new List<Group>();
                if ((bool)SqlCommand.IsChecked)
                {
                    SqlCommand cmd = new SqlCommand("exec sp_columns dic_Group", con);
                    SqlDataReader read = cmd.ExecuteReader();
                    while (read.Read())
                    {

                        GridViewColumn temp = new GridViewColumn();
                        temp.Header = read[3].ToString();
                        temp.DisplayMemberBinding = new Binding(read[3].ToString());
                        GW.Columns.Add(temp);
                    }

                    con.Close();
                    con.Open();

                    SqlCommand com = new System.Data.SqlClient.SqlCommand("select * from dic_Group", con);
                    SqlDataReader read2 = com.ExecuteReader();

                    while (read2.Read())
                    {
                        Group g = new Group();
                        g.GroupId = Int32.Parse(read2[0].ToString());
                        g.Name = read2[1].ToString();
                        lg.Add(g);
                    }
                    DataListView.ItemsSource = lg;
                }

                else if ((bool)DataAdapter.IsChecked)
                {
                    SqlDataAdapter da = new SqlDataAdapter("select*from dic_Group", con);
                    DataTable dt = new DataTable("dic_Model");
                    da.Fill(dt);


                    foreach (DataColumn column in dt.Columns)
                    {

                        GridViewColumn temp = new GridViewColumn();
                        temp.Header = column.ColumnName;
                        temp.DisplayMemberBinding = new Binding(column.ColumnName);
                        GW.Columns.Add(temp);

                    }

                    DataListView.ItemsSource = dt.DefaultView;

                }

                con.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            }

    }

    public class Group
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
    }
}
