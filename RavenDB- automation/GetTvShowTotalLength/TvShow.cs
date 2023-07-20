using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetTvShowTotalLength
{
    public class TvShow
    {
        private int id;
        private string? url;
        private string? name;
        private string? language;
        private string? premiered;
        public TvShow() { }

        public TvShow(int id, string url, string name, string premiered)
        {
            this.Id = id;
            this.Url = url;
            this.Name = name;
            this.Premiered = premiered;
        }

        public int Id { get => id; set => id = value; }
        public string? Url { get => url; set => url = value; }
        public string? Name { get => name; set => name = value; }
        public string? Language { get => language; set => language = value; }
        public string? Premiered { get => premiered; set => premiered = value; }
    }
}
