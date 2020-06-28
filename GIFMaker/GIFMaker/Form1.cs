using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace GIFMaker
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Directory.Exists(@".\image") == false)
            {
                Directory.CreateDirectory(@".\image");
            }
            if (File.Exists(@".\info.txt") == true)
            {
                FileStream f = new FileStream(@".\info.txt", FileMode.Truncate, FileAccess.Write);
                f.Close();
            }
        }

        private void MetroButton1_Click(object sender, EventArgs e)
        {
            //폴더생성
            if (Directory.Exists(@".\video") == false)
            {
                Directory.CreateDirectory(@".\video");
            }
            //info.txt 초기화
            if (File.Exists(@".\info.txt") == true)
            {
                FileStream f = new FileStream(@".\info.txt", FileMode.Truncate, FileAccess.Write);
                f.Close();
            }
            string url = metroTextBox1.Text;
            string fileName;
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "__main__.exe";
                p.StartInfo.Arguments = "-f \"webm[height<=1080]\" " + url;
                if (radioButton2.Checked == true)
                {
                    p.StartInfo.Arguments = "-f \"bestvideo[height<=1080][ext=mp4]\" " + url;
                }
                p.Start();
                p.WaitForExit();
                fileName = getFileName();
                if (fileName == "")
                {
                    System.Windows.Forms.MessageBox.Show("동영상을 다운로드 받지 못했습니다.");
                    return;
                }
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("동영상을 다운로드 받을 수 없습니다.");
                return;
            }
            //파일입출력
            string filePath = ".\\" + fileName;
            string movePath = ".\\video\\" + fileName;
            if (File.Exists(movePath) == false && File.Exists(filePath) == true)
            {
                File.Move(filePath, movePath);
            }
            //파일 이동완료
            if (File.Exists(movePath) == true)
            {
                //Passvalue 제목 movePath 파일 현재위치 imagePath 결과 위치
                Form3 mainForm = new Form3(this);
                mainForm.Passvalue = fileName;
                mainForm.movePath = System.IO.Directory.GetCurrentDirectory() + @"\video\" + fileName;
                mainForm.imagePath = System.IO.Directory.GetCurrentDirectory() + @"\image";
                mainForm.Show();
                this.Hide();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("동영상 파일을 찾지 못했습니다.");
            }
        }

        private void MetroButton2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "파일 찾기";
            openFileDialog.Filter = "mp4 File|*.mp4|webm File|*.webm";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Passvalue 제목 movePath 파일 현재위치 imagePath 결과 위치
                Form3 mainForm = new Form3(this);
                mainForm.Passvalue = Path.GetFileName(openFileDialog.FileName);
                mainForm.movePath = openFileDialog.FileName;
                mainForm.imagePath = System.IO.Directory.GetCurrentDirectory() + @"\image";
                mainForm.Show();
                this.Hide();
            }
        }

        private void MetroTextBox1_Click(object sender, EventArgs e)
        {

        }

        private string getFileName()
        {
            try
            {
                string path = @".\info.txt";
                string[] textValue = System.IO.File.ReadAllLines(path);
                return textValue[0];
            }
            catch (Exception)
            {
                return "";
            }
        }
    }



}
