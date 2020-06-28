using System;
using System.Collections.Generic;
using System.Text;

namespace GIFMaker.Core
{
    public class GifOption
    {
        // 0으로 둘 시 영상의 크기 적용
        public int width { get; set; } = 0;
        public int height { get; set; } = 0;

        // 시간 단위들 전부 (1/1000초)
        public long start { get; set; } = 0;
        public long end { get; set; } = 0;
        public long delay { get; set; } = 0;
    }
}
