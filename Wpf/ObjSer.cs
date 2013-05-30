using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wpf
{
    [Serializable]
    class ObjSer
    {
        List<SerPic> pic = new List<SerPic>();
        List<SerText> text = new List<SerText>();

        public ObjSer()
        {
        }

        public void AddToPicList(SerPic picture)
        {
            pic.Add(picture);
        }

        public void AddToTextList(SerText t)
        {
            text.Add(t);
        }

        public SerPic GetFromPicList(int i)
        {
            return pic[i];
        }

        public SerText GetFromTextList(int i)
        {
            return text[i];
        }

        public int GetPicEnumerator()
        {
            return pic.Count;
        }

        public int GetTextEnumerator()
        {
            return text.Count;
        }
    }
}
