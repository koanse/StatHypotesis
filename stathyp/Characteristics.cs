using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace stathyp
{
    public partial class Characteristics : Form
    {
        public Characteristics(Sample Sample)
        {
            InitializeComponent();
            label1.Text = Sample.Name;
            listView1.Items.Add(new ListViewItem(new string[] { "Объем выборки", Sample.x.Length.ToString() }));
            listView1.Items.Add(new ListViewItem(new string[] { "Среднее", Sample.avx.ToString() }));
            listView1.Items.Add(new ListViewItem(new string[] { "Средний квадрат", Sample.avx2.ToString() }));
            listView1.Items.Add(new ListViewItem(new string[] { "Выборочная дисперсия", Sample.disp.ToString() }));
            listView1.Items.Add(new ListViewItem(new string[] { "Среднее квадратич. откл.", Sample.dev.ToString() }));
            listView1.Items.Add(new ListViewItem(new string[] { "Исправленная дисперсия", Sample.s2.ToString() }));
            listView1.Items.Add(new ListViewItem(new string[] { "Исправленное ср. кв. откл.", Sample.s.ToString() }));
            listView1.Items.Add(new ListViewItem(new string[] { "Минимум", Sample.min.ToString() }));
            listView1.Items.Add(new ListViewItem(new string[] { "Максимум", Sample.max.ToString() }));
            listView1.Items.Add(new ListViewItem(new string[] { "Размах вариации", Sample.r.ToString() }));
            listView1.Items.Add(new ListViewItem(new string[] { "Количество интервалов", Sample.k.ToString() }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}