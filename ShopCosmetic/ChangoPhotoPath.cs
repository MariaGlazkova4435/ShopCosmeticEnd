using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace ShopCosmetic
{
    public class ChangoPhotoPath //изменение пути фото, чтобы отобразить их при смене имени диска или при полной смене пути
    {
        public ChangoPhotoPath()
        {
            NewPath = Process.GetCurrentProcess().MainModule.FileName;
            var folderPath = Path.GetDirectoryName(NewPath);
            for (int i = 0; i < 4; i++)
            {
                var parentFolderPath = Directory.GetParent(folderPath).FullName;
                folderPath = parentFolderPath;
            }
            NewPath = folderPath; //путь к папке приложения
        }
        public string NewPath;
        public ImageSource ByteToImage(byte[] imageData)
        {
            BitmapImage biImg = new BitmapImage();
            MemoryStream ms = new MemoryStream(imageData);
            biImg.BeginInit();
            biImg.StreamSource = ms;
            biImg.EndInit();
            ImageSource imgSrc = biImg;
            return imgSrc;
        }
        //public void Change()
        //{
        //    string search = "\\ShopCosmetic";
        //    var v = Cosmetics.GetContext().Product.ToList();

        //    foreach (var p in v)
        //    {
        //        if (p.photo != null)
        //        {
        //            string input = p.photo;
        //            int index = input.IndexOf(search);
        //            if (index > -1)
        //            {
        //                string result = input.Substring(0, index); //ее надо убрать
        //                var output = input.Replace(result, NewPath);
        //                p.photo = output;
        //            }
        //        }
        //    }
        //    var b = Cosmetics.GetContext().TypePurpose.ToList();
        //    foreach (var p in b)
        //    {
        //        if (p.photo != null)
        //        {
        //            string input = p.photo;
        //            int index = input.IndexOf(search);
        //            if (index > -1)
        //            {
        //                string result = input.Substring(0, index); //ее надо убрать
        //                var output = input.Replace(result, NewPath);
        //                p.photo = output;
        //            }
        //        }
        //    }
        //    var c = Cosmetics.GetContext().OtherPhoto.ToList();
        //    foreach (var p in c)
        //    {
        //        if (p.Path != null)
        //        {
        //            string input = p.Path;
        //            int index = input.IndexOf(search);
        //            if (index > -1)
        //            {
        //                string result = input.Substring(0, index); //ее надо убрать
        //                var output = input.Replace(result, NewPath);
        //                p.Path = output;
        //            }
        //        }
        //    }

        //    Cosmetics.GetContext().SaveChanges();
        //}
    }
}
