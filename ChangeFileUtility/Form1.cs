using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChangeFileUtility
{
    public partial class Form1 : Form
    {
        public string _OldFileName = string.Empty;
        public string _NewFileName = string.Empty;
        public string _FileFormat = string.Empty;
        public Form1()
        {
            InitializeComponent();
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            label3.Visible = false;
            linkLabel1.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            _NewFileName = openFileDialog1.FileName;
            textBox1.Text = _NewFileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            _OldFileName = openFileDialog1.FileName;
            textBox2.Text = _OldFileName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (_NewFileName.Length == 0)
            {
                label3.Visible = true;
                label3.Text = "Missing New File";
                return;
            }
            else if (_OldFileName.Length == 0)
            {
                label3.Visible = true;
                label3.Text = "Missing old File";
                return;
            }
            else if (_FileFormat.Length == 0)
            {
                label3.Visible = true;
                label3.Text = "Missing old File";
                return;
            }

            FileCompare(_FileFormat.ToUpper());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _FileFormat = comboBox1.SelectedItem.ToString();
        }
        public void FileCompare(string _FileFormat)
        {
            try
            {
                string sNewerFilePath = _NewFileName;
                string sOlderFilePath = _OldFileName;
                string[] _newFile = File.ReadAllLines(sNewerFilePath);
                string[] _oldFile = File.ReadAllLines(sOlderFilePath);
                List<string> _DeltaFile = new List<string>();
                List<string> _AddedEmplList = new List<string>();

                List<string> _newFileUpper = _newFile.ToList().Select(x => x.ToUpper()).ToList();
                List<string> _oldFileUpper = _oldFile.ToList().Select(x => x.ToUpper()).ToList();

                //var deltaData = _newFileUpper.ToArray().Except(_oldFileUpper.ToArray());
                var deltaData = _newFileUpper.Union(_oldFileUpper).Except(_newFileUpper.Intersect(_oldFileUpper));
                //foreach (var _line in _newFile)
                //{
                //    if (!_oldFile.ToArray().Where(x=> x == _line.ToString()).Any())
                //    {
                //        string EmplID = ExtractEmplID(_line, _FileFormat);
                //        foreach (var MatchedLine in _newFile.ToList().Where(x => x.Contains(EmplID)))
                //        {
                //            if (MatchedLine.IndexOf(EmplID) < 15)
                //            { 
                //                if (!_AddedEmplList.Contains(EmplID))
                //                {
                //                    _DeltaFile.Add(MatchedLine);
                //                }
                //            }
                //        }
                //        _AddedEmplList.Add(EmplID);
                //    }
                //}

                foreach (var _line in deltaData)
                {
                    string EmplID = ExtractEmplID(_line, _FileFormat);
                    foreach (var MatchedLine in _newFile.ToList().Where(x => x.Contains(EmplID)))
                    {
                        if (MatchedLine.IndexOf(EmplID) < 15)
                        {
                            if (!_AddedEmplList.Contains(EmplID))
                            {
                                _DeltaFile.Add(MatchedLine);
                            }
                        }
                    }
                    _AddedEmplList.Add(EmplID);
                }
                string _DeltaFileName = DeltaFileName(_NewFileName);
                File.WriteAllLines(_DeltaFileName, _DeltaFile.ToArray());
                linkLabel1.Visible = true;
                linkLabel1.Text = _DeltaFileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public string ExtractEmplID(string _lineVal, string _FileFormat)
        {
            if (_FileFormat == "STANDARDTXT")
            {
                return StandardTxtReturn(_lineVal);
            }
            else if (_FileFormat == "STANDARDCSV")
            {
                return StandardCSVReturn(_lineVal);
            }
            else if (_FileFormat == "BLUESKYCSV")
            {
                return BlueSkyCSVReturn(_lineVal);
            }
            return string.Empty;
        }
        public string DeltaFileName(string OriginalFileName)
        {
             string _NewFileName = OriginalFileName.Split('\\').LastOrDefault().ToString();
            string _Extension = OriginalFileName.Substring(OriginalFileName.IndexOf("."));
            string _AppendedName = "_Modified" + DateTime.Now.ToFileTime();
            _NewFileName = _NewFileName.Replace(_Extension, _AppendedName) + _Extension;
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var FinalFilePath = Path.Combine(path, _NewFileName);
            return FinalFilePath;
        }
        public string StandardTxtReturn(string _lineVal)
        {
            var EmplID = string.Empty;
            if (_lineVal.StartsWith("EMPL"))
            {
                EmplID = _lineVal.Substring(8, 9);
            }
            else if (_lineVal.StartsWith("DPDT") || _lineVal.StartsWith("SPSE") || _lineVal.StartsWith("BENE"))
            {
                EmplID = _lineVal.Substring(5, 9);
            }
            else
            {
                EmplID = _lineVal.Substring(9, 9);
            }
            return EmplID;
        }
        public string StandardCSVReturn(string _lineVal)
        {
            var EmplID = string.Empty;
            if (_lineVal.Contains("DPDT") || _lineVal.Contains("SPSE"))
            {
                EmplID = _lineVal.Split(',')[2].ToString();
            }
            else
            {
                EmplID = _lineVal.Split(',')[3].ToString();
            }
            return EmplID;
        }
        public string BlueSkyCSVReturn(string _lineVal)
        {
            var EmplID = _lineVal.Split(',')[2].ToString();
            if (EmplID.Length == 0)
            {
                EmplID = _lineVal.Split(',')[3].ToString();
            }
            return EmplID;
        }
    }

}

