using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace stathyp
{
    public partial class MainForm : Form
    {
        ArrayList Samples;
        Sample SelectedSample;
        Sample CurrentSample;
        float alpha = 0.01f;
        public MainForm()
        {
            InitializeComponent();
            toolStripContainer1.TopToolStripPanelVisible = false;
            toolStripContainer1.RightToolStripPanelVisible = false;
            toolStripContainer1.BottomToolStripPanelVisible = false;
            Samples = new ArrayList();
        }
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void CreateToolStrip(Sample s)
        {
            ToolStrip ts = new ToolStrip();
            ToolStripLabel tsl1 = new ToolStripLabel();
            ToolStripLabel tsl2 = new ToolStripLabel();
            ToolStripLabel tsl3 = new ToolStripLabel();
            ToolStripSeparator tss = new ToolStripSeparator();

            ts.ContextMenuStrip = contextMenuStrip1;
            ts.Dock = DockStyle.Top;
            ts.Items.AddRange(new ToolStripItem[] { tsl1, tss, tsl2, tsl3 });
            ts.Parent = toolStripContainer1.LeftToolStripPanel;
            tsl1.Text = s.Name;
            tsl2.Text = "Объем " + s.x.Length.ToString();
            tsl3.Text = "Среднее " + string.Format("{0:F3}", s.avx);
            ts.Click += new EventHandler(ts_Click);
            ts.MouseEnter += new EventHandler(ts_MouseEnter);
            s.ts = ts;
        }
        void ts_MouseEnter(object sender, EventArgs e)
        {
            foreach (Sample s in Samples)
                if (s.Name == (sender as ToolStrip).Items[0].Text)
                {
                    CurrentSample = s;
                    break;
                }
        }
        void ts_Click(object sender, EventArgs e)
        {
            foreach (Sample s in Samples)
                if (s.Name == (sender as ToolStrip).Items[0].Text)
                {
                    SelectedSample = s;
                    toolStripContainer1.ContentPanel.Invalidate();
                    break;
                }
        }
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;
            else e.Effect = DragDropEffects.None;
        }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            char[] sep = new char[] { '\t', '\r', ' ', '\n' };
            string data = e.Data.GetData(DataFormats.Text).ToString();
            string[] splitted = data.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            ArrayList x = new ArrayList();
            foreach (string s in splitted)
            {
                try
                {
                    float tmp = float.Parse(s);
                    if (IgnoreZeroesToolStripMenuItem.Checked && tmp == 0) continue;
                    x.Add(tmp);
                }
                catch
                {
                    MessageBox.Show("Неправильный формат", "Ошибка перевода");
                }
            }

            if (x.Count == 0) return;
            int num = 1;
            if (Samples.Count != 0)
                while (true)
                {
                    bool Found = false;
                    foreach (Sample s in Samples)
                        if (s.Name == "Выборка " + num.ToString())
                        {
                            Found = true;
                            break;
                        }
                    if (Found == false) break;
                    num++;
                }
            
            Sample smpl = new Sample("Выборка " + num.ToString(),
                x.ToArray(typeof(float)) as float[]);
            CreateToolStrip(smpl);
            Samples.Add(smpl);
            SelectedSample = smpl;
            toolStripContainer1.ContentPanel.Invalidate();
        }
        private void toolStripContainer1_ContentPanel_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                RectangleF DrawPlate = toolStripContainer1.ContentPanel.ClientRectangle;
                RectangleF DrawArea = toolStripContainer1.ContentPanel.ClientRectangle;
                RectangleF ForString = new RectangleF();
                Pen BlackPen = new Pen(Color.Black, 3);
                Pen RedPen = new Pen(Color.Red, 6);
                SolidBrush WhiteBrush = new SolidBrush(Color.White);
                SolidBrush BlueBrush = new SolidBrush(Color.Blue);
                SolidBrush BlackBrush = new SolidBrush(Color.Black);
                Font Font = new Font("Arial", 8);

                DrawPlate.X += 10;
                DrawPlate.Y += 30;
                DrawPlate.Width -= 20;
                DrawPlate.Height -= 60;
                
                DrawArea.X += 20;
                DrawArea.Y += 60;
                DrawArea.Width -= 40;
                DrawArea.Height -= 120;

                ForString.X = DrawArea.X + 5;
                ForString.Y = DrawArea.Y + DrawArea.Height;
                ForString.Height = 15;
                ForString.Width = DrawArea.Width / SelectedSample.k;
                
                e.Graphics.FillRectangle(WhiteBrush, DrawPlate);
                e.Graphics.DrawString(string.Format("{0:F3}", SelectedSample.Name),
                    Font, BlackBrush, DrawPlate.X, DrawPlate.Y);

                RectangleF[] Rects = new RectangleF[SelectedSample.k];
                for (int i = 0; i < SelectedSample.k; i++)
                {
                    float x = DrawArea.X + i * DrawArea.Width / SelectedSample.k;
                    float y = DrawArea.Y + DrawArea.Height * (1 - (float) SelectedSample.n[i] / SelectedSample.maxn);
                    float width = DrawArea.Width / SelectedSample.k;
                    float height = DrawArea.Height * (float) SelectedSample.n[i] / SelectedSample.maxn;
                    Rects[i] = new RectangleF(x, y, width, height);
                    ForString.X = x + 5;
                    float tmp = (SelectedSample.bounds[i] + SelectedSample.bounds[i + 1]) / 2;
                    e.Graphics.DrawString(string.Format("{0:F3}", tmp), Font, BlackBrush, ForString);
                }
                e.Graphics.FillRectangles(BlueBrush, Rects);
                e.Graphics.DrawRectangles(BlackPen, Rects);
            }
            catch
            {
            }
        }
        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentSample.ts.Dispose();
            Samples.Remove(CurrentSample);
            if (CurrentSample == SelectedSample)
            {
                SelectedSample = null;
                toolStripContainer1.ContentPanel.Invalidate();
            }
        }
        private void CharacteristicsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Characteristics form = new Characteristics(CurrentSample);
            form.ShowDialog();
        }
        private void проверкаОднородностиНесколькихВыборокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResultForm form = new ResultForm();
            int num = 0;
            bool res = true;
            string[] tmp = new string[7];
            for (int i = 0; i < Samples.Count; i++)
            {
                Sample s1 = Samples[i] as Sample;
                for (int j = i + 1; j < Samples.Count; j++)
                {
                    Sample s2 = Samples[j] as Sample;
                    Criterions c = new Criterions(s1.x, s2.x, 0.05f);

                    num++;                    
                    tmp[0] = s1.Name;
                    tmp[1] = s2.Name;
                    c.F(CriticalAreaType.DoubleSided);
                    if (c.result == true)
                    {
                        tmp[2] = "Однородны";
                        tmp[3] = "Однородны";
                        tmp[4] = "Однородны";
                        tmp[5] = "Однородны";
                        tmp[6] = "Однородны";
                    }
                    else
                    {
                        tmp[2] = "Не однородны";
                        tmp[3] = "Не однородны";
                        tmp[4] = "Не однородны";
                        tmp[5] = "Не однородны";
                        tmp[6] = "Не однородны";
                        res = false;
                    }
                    if (res) form.label2.Text = "объединенная выборка однородна с вероятностью p = "
                        + string.Format("{0:F4}", 1 - alpha * num) + ".";
                    else form.label2.Text = "объединенная выборка не однородна с вероятностью p = "
                        + string.Format("{0:F4}", 1 - alpha * num) + ".";

                    //c.W(CriticalAreaType.DoubleSided, 50);
                    //if (c.result == true) tmp[3] = "Однородны";
                    //else
                    //{
                    //    tmp[3] = "Не однородны";
                    //    form.label2.Text = "объединенная выборка не является однородной.";
                    //}
                    /*c.Q();
                    if (c.result == true) tmp[4] = "Однородны";
                    else
                    {
                        tmp[4] = "Не однородны";
                        form.label2.Text = "объединенная выборка не является однородной.";
                    }*/
                    //c.A();
                    //if (c.result == true) tmp[5] = "Однородны";
                    //else
                    //{
                    //    tmp[5] = "Не однородны";
                    //    form.label2.Text = "объединенная выборка не является однородной.";
                    //}

                    form.listView1.Items.Add(new ListViewItem(tmp));
                }
            }
            form.ShowDialog();
        }
        private void IgnoreZeroesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(IgnoreZeroesToolStripMenuItem.Checked)
                IgnoreZeroesToolStripMenuItem.Checked = false;
            else IgnoreZeroesToolStripMenuItem.Checked = true;
        }
        private void toolStripTextBox1_Leave(object sender, EventArgs e)
        {
            try
            {
                float tmp = float.Parse(toolStripTextBox1.Text);
                alpha = tmp;
            }
            catch
            {
            }

        }
        private void toolStripTextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                float tmp = float.Parse(toolStripTextBox1.Text);
                alpha = tmp;
            }
            catch
            {
                alpha = 0.01f;
                toolStripTextBox1.Text = string.Format("{0:F3}", alpha);
            }
        }
        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About form = new About();
            form.ShowDialog();
        }
    }
    public class Sample
    {
        public ToolStrip ts;
        public string Name;
        public float[] x;
        public float avx;
        public float avx2;
        public float disp;
        public float dev;
        public float s;
        public float s2;
        public float max;
        public float min;
        public float r;

        public int k;
        public float[] bounds;
        public int[] n;
        public int maxn;

        public Sample(string name, float[] array)
        {
            float sumx = 0, sumx2 = 0;
            Name = name;
            x = array;
            max = min = x[0];
            foreach (float xi in x)
            {
                sumx += xi;
                sumx2 += xi * xi;
                if (max < xi) max = xi;
                if (min > xi) min = xi;
            }
            r = max - min;
            avx = sumx / x.Length;
            avx2 = sumx2 / x.Length;
            disp = avx2 - avx * avx;
            dev = (float)Math.Pow(disp, 0.5);
            s2 = disp * x.Length / (x.Length - 1);
            s = (float)Math.Pow(s2, 0.5);

            // Формула Старджеса
            k = (int)Math.Round(Math.Log(x.Length, 2)) + 1;
            
            // Границы интервалов
            bounds = new float[k + 1];
            bounds[0] = min;
            bounds[k] = max;
            for (int i = 1; i < k; i++)
                bounds[i] = bounds[i - 1] + r / k;

            // Частоты
            n = new int[k];
            foreach (float xi in x)
                for (int j = 0; j < k; j++)
                    if (xi >= bounds[j] && xi <= bounds[j + 1])
                    {
                        n[j]++;
                        break;
                    }

            maxn = 0;
            foreach (int ni in n)
                if (ni > maxn) maxn = ni;
        }
    }
}