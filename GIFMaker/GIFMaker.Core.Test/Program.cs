using System;
using GIFMaker.Core;

namespace GIFMaker.Core.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var creator = new VideoManager(@"C:\Users\trick\Desktop\1.mp4"))
            {
                // SaveGIF를 이용한 GIF 저장 가능
                GifOption option = new GifOption();
                option.delay = 1000 / 15;
                option.start = 180 * 1000;
                option.end = 190 * 1000;
                option.width = creator.width / 2;
                option.height = creator.height / 2;

                creator.SaveGif(option, "test.gif");

                // Seek 및 NextBitmapFrame을 이용한 비트맵 받아오기 및 영상 재생 가능
                creator.Seek(185 * 1000);
                var bmp = creator.NextBitmapFrame().bitmap;
                bmp.Save("test.png");
            }
        }
    }
}
