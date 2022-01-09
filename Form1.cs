using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WordleSolver
{
    public partial class Form1 : Form
    {
        TextBox f;
        TextBox ib, fb;
        List<TextBox> _iy, _fy;
        List<TextBox> _ig, _fg;
        TextBox[] _iCon;
        TextBox[] _fCon;

        Pen selPen;
        Rectangle selRec;

        string[] _dic0;

        public Form1()
        {
            InitControls();
            InitEvents();

            InitializeComponent();

            CalculateList();
        }

        private void InitEvents()
        {
            this.Resize += Form1_Resize;
            this.Paint += Form1_Paint;     

            foreach (TextBox tb in _iCon)
            {
                tb.KeyDown += Tb_KeyDown;
                tb.GotFocus += Tb_GotFocus;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(selPen, selRec);
        }

        private void Tb_GotFocus(object sender, EventArgs e)
        {
            Control aCon = this.ActiveControl;

            for (int i = 0; i < _iCon.Length; i++)
            {
                if (aCon == _iCon[i])
                {
                    selRec = new Rectangle(_iCon[i].Location, _iCon[i].Size);
                    this.Invalidate();
                }
            }
        }

        private void Tb_KeyDown(object sender, KeyEventArgs e)
        {
            Control aCon = this.ActiveControl;

            if (e.KeyCode == Keys.Enter)
            {
                for (int i = 0; i < _iCon.Length; i++)
                {
                    if (aCon == _iCon[i])
                    { SubmitToFeed(_iCon[i], _fCon[i]); }
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                if (aCon == ib) { this.ActiveControl = _iy[0]; }

                for (int i = 0; i < _iy.Count; i++)
                { if (aCon == _iy[i]) { this.ActiveControl = _ig[i]; } }
            }
            else if (e.KeyCode == Keys.Left)
            {
                for (int i = 0; i < _ig.Count; i++)
                {
                    if (aCon == _ig[i]) { this.ActiveControl = _iy[i]; }
                    if (aCon == _iy[i]) { this.ActiveControl = ib; }
                }
            }
            else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                if(e.KeyCode == Keys.Up)
                {
                    for (int i = 2; i < _iCon.Length; i++)
                    { if (aCon == _iCon[i]) { this.ActiveControl = _iCon[i - 1]; } }
                }

                if (e.KeyCode == Keys.Down)
                {
                    for (int i = 1; i < _iCon.Length-1; i++)
                    { if (aCon == _iCon[i]) { this.ActiveControl = _iCon[i + 1]; } }
                }
            }
        }
        private void SubmitToFeed(TextBox _iCon, TextBox _fCon)
        {
            if(_iCon.Text == "") { return; }

            if (_ig.IndexOf(_iCon) > -1)
            { 
                _fCon.Text =
                    _fCon.Text.IndexOf(_iCon.Text) == -1 ?
                    (_iCon.Text) :
                    _fCon.Text.Replace(_iCon.Text, "").Replace(",,", ",");
            }
            else
            {
                _fCon.Text =
                _fCon.Text.IndexOf(_iCon.Text) == -1 ?
                (_fCon.Text + ',' + _iCon.Text) :
                _fCon.Text.Replace(_iCon.Text, "").Replace(",,", ",");
            }

            _fCon.Text = _fCon.Text.Trim(',');
            _iCon.Text = "";

            CalculateList();
        }

        private void CalculateList()
        {
            IEnumerable<string> ie_dic1 = _dic0.Select(x => x);
            IEnumerable<string> ie_dic1Sorted;

            FilterBlack(ref ie_dic1);
            FilterGreen(ref ie_dic1);
            FilterYellow(ref ie_dic1);

            ie_dic1Sorted = SortDic(ie_dic1);

            f.Text = String.Concat(ie_dic1Sorted.Select(x => x + Environment.NewLine).ToArray());
        }

        private IEnumerable<string> SortDic(IEnumerable<string> ie_dic1)
        {
            IEnumerable<char>[] _dicPos = new IEnumerable<char>[5];

            _dicPos[0] = ie_dic1.Select(x => x[0]);
            _dicPos[1] = ie_dic1.Select(x => x[1]);
            _dicPos[2] = ie_dic1.Select(x => x[2]);
            _dicPos[3] = ie_dic1.Select(x => x[3]);
            _dicPos[4] = ie_dic1.Select(x => x[4]);

            return ie_dic1
                .Select(x => x + " " + (
                    _dicPos[0].Where(y => y == x[0]).Count() +
                    _dicPos[1].Where(y => y == x[1]).Count() +
                    _dicPos[2].Where(y => y == x[2]).Count() +
                    _dicPos[3].Where(y => y == x[3]).Count() +
                    _dicPos[4].Where(y => y == x[4]).Count()
                    ).ToString()
                )
                .OrderByDescending(x => Int32.Parse(x.Substring(5)));
        }

        private void FilterBlack(ref IEnumerable<string> ie_dic1)
        {
            if (fb.Text != "")
            {
                foreach (string str in fb.Text.Split(','))
                { ie_dic1 = ie_dic1.Where(x => x.IndexOf(str) == -1); }
            }
        }
        private void FilterGreen(ref IEnumerable<string> ie_dic1)
        {
            if (_fg[0].Text != "") { ie_dic1 = ie_dic1.Where(x => x[0].ToString() == _fg[0].Text); }
            if (_fg[1].Text != "") { ie_dic1 = ie_dic1.Where(x => x[1].ToString() == _fg[1].Text); }
            if (_fg[2].Text != "") { ie_dic1 = ie_dic1.Where(x => x[2].ToString() == _fg[2].Text); }
            if (_fg[3].Text != "") { ie_dic1 = ie_dic1.Where(x => x[3].ToString() == _fg[3].Text); }
            if (_fg[4].Text != "") { ie_dic1 = ie_dic1.Where(x => x[4].ToString() == _fg[4].Text); }
        }
        private void FilterYellow(ref IEnumerable<string> ie_dic1)
        {
            if (_fy[0].Text != "")
            {
                foreach (string str in _fy[0].Text.Split(','))
                { ie_dic1 = ie_dic1.Where(x => x[0].ToString() != str && x.IndexOf(str) != -1); }
            }
            if (_fy[1].Text != "")
            {
                foreach (string str in _fy[1].Text.Split(','))
                { ie_dic1 = ie_dic1.Where(x => x[1].ToString() != str && x.IndexOf(str) != -1); }
            }
            if (_fy[2].Text != "")
            {
                foreach (string str in _fy[2].Text.Split(','))
                { ie_dic1 = ie_dic1.Where(x => x[2].ToString() != str && x.IndexOf(str) != -1); }
            }
            if (_fy[3].Text != "")
            {
                foreach (string str in _fy[3].Text.Split(','))
                { ie_dic1 = ie_dic1.Where(x => x[3].ToString() != str && x.IndexOf(str) != -1); }
            }
            if (_fy[4].Text != "")
            {
                foreach (string str in _fy[4].Text.Split(','))
                { ie_dic1 = ie_dic1.Where(x => x[4].ToString() != str && x.IndexOf(str) != -1); }
            }
        }

            private void Form1_Resize(object sender, EventArgs e)
        {
            int wSpace = 10;
            int w0 = 50;
            int w1 = (this.ClientSize.Width - 4 * w0 - 3 * wSpace) / 3;

            foreach (TextBox tb in new TextBox[] { ib }.Concat(_iy).Concat(_ig).Concat(_fg).ToArray())
            { tb.Width = w0; }

            foreach (TextBox tb in new TextBox[] { f, fb }.Concat(_fy).ToArray())
            { tb.Width = w1; }

            foreach (TextBox tb in new TextBox[] { f, fb })
            { tb.Height = this.ClientSize.Height; }

            f.Location = new Point(0, 0);
            ib.Location = new Point(f.Location.X + f.Width + wSpace, 0);
            fb.Location = new Point(ib.Location.X + ib.Width, 0);

            for(int i = 0; i < _iy.Count; i++)
            {
                _iy[i].Location = new Point(fb.Location.X + fb.Width + wSpace, ib.Height * i);
                _fy[i].Location = new Point(_iy[i].Location.X + _iy[i].Width, ib.Height * i);
                _ig[i].Location = new Point(_fy[i].Location.X + _fy[i].Width + wSpace, ib.Height * i);
                _fg[i].Location = new Point(_ig[i].Location.X + _ig[i].Width, ib.Height * i);
            }
        }
        private void InitControls()
        {
            double iFac = 1.3;
            Color colfb = Color.FromArgb(58, 58, 60);
            Color colfy = Color.FromArgb(181, 159, 59);
            Color colfg = Color.FromArgb(83, 141, 78);
            Color colib = Color.FromArgb((int)(colfb.R * iFac), (int)(colfb.G * iFac), (int)(colfb.B * iFac));
            Color coliy = Color.FromArgb((int)(colfy.R * iFac), (int)(colfy.G * iFac), (int)(colfy.B * iFac));
            Color colig = Color.FromArgb((int)(colfg.R * iFac), (int)(colfg.G * iFac), (int)(colfg.B * iFac));

            selPen = new Pen(Color.Black, 3);
            _dic0 = File.ReadAllLines(@"C:\Users\Dan Roe\source\repos\WordleSolver\bin\dictionary5.txt");

            f = new TextBox();
            ib = new TextBox();
            fb = new TextBox();
            _iy = new List<TextBox>();
            _fy = new List<TextBox>();
            _ig = new List<TextBox>();
            _fg = new List<TextBox>();

            foreach (TextBox tb in new TextBox[] { f, fb, ib })
            { this.Controls.Add(tb); }

            f.BackColor = Color.DarkGray;
            ib.BackColor = colib;
            ib.ForeColor = Color.White;
            fb.BackColor = colfb;
            fb.ForeColor = Color.White;

            for (int i = 0; i < 5; i++)
            {
                _iy.Add(new TextBox());
                _fy.Add(new TextBox());
                _ig.Add(new TextBox());
                _fg.Add(new TextBox());

                this.Controls.Add(_fy[i]);
                this.Controls.Add(_iy[i]);
                this.Controls.Add(_ig[i]);
                this.Controls.Add(_fg[i]);

                _fy[i].BackColor = colfy;
                _iy[i].BackColor = coliy;
                _ig[i].BackColor = colig;
                _fg[i].BackColor = colfg;
            }

            _iCon = new TextBox[] { ib }.Concat(_iy).Concat(_ig).ToArray();
            _fCon = new TextBox[] { fb }.Concat(_fy).Concat(_fg).ToArray();

            foreach (TextBox tb in _iCon)
            { tb.MaxLength = 1; }

            foreach (TextBox tb in _fCon)
            { tb.ReadOnly = true; }

            foreach (TextBox tb in new TextBox[] { f, fb })
            {
                tb.Multiline = true;
                tb.ScrollBars = ScrollBars.Vertical;
            }

            foreach (TextBox tb in this.Controls)
            {
                tb.Visible = true;
                tb.CharacterCasing = CharacterCasing.Upper;
            }
        }
    }
}
