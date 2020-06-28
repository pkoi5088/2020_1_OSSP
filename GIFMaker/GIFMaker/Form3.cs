using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GIFMaker.Core;
using System.Threading;
using System.Diagnostics;

namespace GIFMaker
{

    public partial class Form3 : MetroFramework.Forms.MetroForm
    {
        private VideoManager vManager = null;
        private Form1 parentForm;

        private string title;
        private string filePath;
        private string outputPath;
        //Passvalue 제목 movePath 파일 현재위치 imagePath 결과 위치
        public string Passvalue
        {
            get { return this.title; }
            set { this.title = value; }
        }
        public string movePath
        {
            get { return this.filePath; }
            set { this.filePath = value; }
        }
        public string imagePath
        {
            get { return this.outputPath; }
            set { this.outputPath = value; }
        }

        bool playing = false;
        bool stopPlay = false;

        public Form3(Form1 parentForm)
        {
            this.parentForm = parentForm;
            InitializeComponent();
            //dateTimePicker1.ShowUpDown = true;
            
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams handleparam = base.CreateParams;
                handleparam.ExStyle |= 0x02000000;
                return handleparam;
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            metroLabel1.Text = title;//폼1에서 입력받은 URL을 받아옴.
            metroLabel2.Text = outputPath;//폼1에서 입력받은 URL을 받아옴.
            try
            {
                if (vManager != null)
                    vManager.Dispose();

                vManager = new VideoManager(filePath);
                numericUpDown1.Maximum = vManager.duration;
                numericUpDown2.Maximum = vManager.duration;
                numericUpDown3.Maximum = vManager.width;
                numericUpDown4.Maximum = vManager.height;
                numericUpDown5.Maximum = 30;
                numericUpDown5.Minimum = 1;
                numericUpDown1.Value = 0;
                numericUpDown2.Value = Convert.ToDecimal((double)vManager.duration / 1000);
                numericUpDown3.Value = Convert.ToDecimal(vManager.width);
                numericUpDown4.Value = Convert.ToDecimal(vManager.height);
                numericUpDown5.Value = 15;
            }
            catch(Exception)
            {
                System.Windows.Forms.MessageBox.Show("Form3_Load에서 예외발생");
                Application.ExitThread();
                Environment.Exit(0);
            }
        }

        private void MetroButton1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBDialog = new FolderBrowserDialog();
            if (FBDialog.ShowDialog() == DialogResult.OK)
            {
                outputPath = FBDialog.SelectedPath;
                metroLabel2.Text = outputPath;//폼1에서 입력받은 URL을 받아옴.
            }
        }

        private void MetroButton2_Click(object sender, EventArgs e)
        {
            long start = (long)(Convert.ToDouble(numericUpDown1.Value) * 1000);
            long end = (long)(Convert.ToDouble(numericUpDown2.Value) * 1000);
            int w = Convert.ToInt32(numericUpDown3.Value);
            int h = Convert.ToInt32(numericUpDown4.Value);
            int fps = Convert.ToInt32(numericUpDown5.Value);

            // SaveGIF를 이용한 GIF 저장 가능
            GifOption option = new GifOption();
            option.delay = 1000 / fps;
            option.start = start;
            option.end = end;
            option.width = w;
            option.height = h;

            vManager.SaveGif(option, outputPath + '\\' + title + ".gif");
            System.Windows.Forms.MessageBox.Show("생성완료");
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown1.Value > numericUpDown2.Value)
            {
                numericUpDown1.Value = numericUpDown2.Value;
            }
        }

        private async void MetroButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (playing)
                {
                    stopPlay = true;
                    while (playing)
                    {
                        await Task.Delay(100);
                    }
                }

                stopPlay = false;
                playing = true;

                Stopwatch stopWatch = new Stopwatch();

                long start = (long)(Convert.ToDouble(numericUpDown1.Value) * 1000);
                long end = (long)(Convert.ToDouble(numericUpDown2.Value) * 1000);

                vManager.Seek(start);

                {
                    var frame = vManager.NextBitmapFrame();
                    if (frame == null)
                    {
                        playing = false;
                        return;
                    }

                    start = (long)frame.pts;

                    metroPanel1.BackgroundImage = null;
                    metroPanel1.Refresh();

                    stopWatch.Start();
                    metroPanel1.BackgroundImage = frame.bitmap;
                    metroPanel1.Refresh();
                }

                while (!stopPlay)
                {
                    var frame = vManager.NextBitmapFrame();
                    if (frame == null || (long)frame.pts > end)
                    {
                        break;
                    }

                    long toWait = (long)frame.pts - start;

                    while (toWait > stopWatch.ElapsedMilliseconds)
                    {
                        Thread.Sleep(1);
                        Application.DoEvents();
                    }

                    //Thread.Sleep((int)(toWait - stopWatch.ElapsedMilliseconds));

                    metroPanel1.BackgroundImage = frame.bitmap;
                    metroPanel1.Refresh();
                }
                playing = false;
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("동영상 재생을 중지합니다.");
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                numericUpDown3.Value = Convert.ToDecimal(vManager.width);
                numericUpDown4.Value = Convert.ToDecimal(vManager.height);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked == true)
            {
                numericUpDown3.Value = Convert.ToDecimal(vManager.width / 2);
                numericUpDown4.Value = Convert.ToDecimal(vManager.height / 2);
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
            {
                numericUpDown3.Value = Convert.ToDecimal((int)(vManager.width * 0.3));
                numericUpDown4.Value = Convert.ToDecimal((int)(vManager.height * 0.3));
            }
        }

        private async void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (vManager != null)
                vManager.Dispose();

            if (playing)
            {
                stopPlay = true;
                while (playing)
                {
                    await Task.Delay(100);
                }
            }
            stopPlay = false;

            parentForm.Show();
            this.Dispose();
        }

    }
}
