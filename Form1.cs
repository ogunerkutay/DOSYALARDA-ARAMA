using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DOSYALARDA_ARAMA
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        string filepath1;
        List<string> filepaths;


        private void button1_Click(object sender, EventArgs e)
        {
            DosyaSec(out filepath1);

            if (filepath1 != null)
            {
                textBox1.Text = filepath1;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DosyalarSec(out filepaths);

            if (filepaths != null)
            {
                foreach (string paths in filepaths)
                {
                    textBox2.Text += paths + "\r\n";
                }
            }

        }

        /// <summary>
        /// Dosya seçimi yaptırır ve dosya yolunu out parametre olarak verir
        /// </summary>
        /// <param name="filePath"></param>
        void DosyaSec(out string filePath)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Metin Dosyaları(*txt)|*.txt";

            DialogResult result = ofd.ShowDialog();

            if (result == DialogResult.OK)
            {
                filePath = ofd.FileName;
            }
            else
            {
                filePath = null;
            }
        }

        /// <summary>
        /// Birden fazla dosya seçimi yaptırır
        /// </summary>
        /// <param name="filePath"></param>
        void DosyalarSec(out List<string> filePaths)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"D:\",
                Title = "Dosyaları Seçiniz",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 0,
                RestoreDirectory = true,
                Multiselect = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                filePaths = openFileDialog1.FileNames.ToList();

                if (filePaths.Contains(filepath1))
                {
                    filepaths.Remove(filepath1);
                }
            }
            else
            {
                filePaths = null;
            }

        }

        /// <summary>
        /// Verilen dosya yolundaki dosyayı okur ve küçük karakterlere çevirir, noktalama işaretlerinden arındırarak dizi haline getirir
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="punctuation"></param>
        /// <returns></returns>
        string[] DosyaOku(string filePath)
        {
            string read = File.ReadAllText(filePath);
            read = read.ToLower();
            var punctuation = read.Where(Char.IsPunctuation).Distinct().ToArray();
            string[] fileWords = read.Split().Select(x => x.Trim(punctuation)).ToArray();
            //string[] fileWorlds = read.Split(punctuation.ToCharArray()); //punctuationlardan dolayı alt satıra geçişlerde hatalı çalışıyor
            return fileWords;
        }

        /// <summary>
        /// Verilen dosyaları okur ve küçük karakterlere çevirir, noktalama işaretlerinden arındırarak dizi haline getirir
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="punctuation"></param>
        /// <returns></returns>
        List<string[]> DosyalarOku(List<string> filePaths)
        {
            List<string[]> fileWords = new List<string[]>();
            foreach (string paths in filePaths)
            {
                string read = File.ReadAllText(paths);
                read = read.ToLower();
                var punctuation = read.Where(Char.IsPunctuation).Distinct().ToArray();
                fileWords.Add(read.Split().Select(x => x.Trim(punctuation)).ToArray());
            }
            return fileWords;
        }

        /// <summary>
        /// Verilen string[] ifadedeki kelimeleri boşluklardan arındırarak geri döndürür
        /// </summary>
        /// <param name="fileWords"></param>
        /// <returns></returns>
        List<string> DosyaKelimeleriniListele(string[] fileWords)
        {
            List<string> fileWordsWithOutEmptyString = new List<string>();
            foreach (var word in fileWords)
            {
                if (!string.IsNullOrEmpty(word))
                {
                    fileWordsWithOutEmptyString.Add(word);
                }
            }
            return fileWordsWithOutEmptyString;
        }


        /// <summary>
        /// Verilen List<string[]> ifadedeki kelimeleri boşluklardan arındırarak listeler. Tekrar eden kelimeler bulundurmaz.
        /// </summary>
        /// <param name="fileWords"></param>
        /// <returns></returns>
        HashSet<string> UniqueDosyaKelimeleriniListele(List<string[]> fileWords)
        {
            HashSet<string> fileWordsWithOutEmptyString = new HashSet<string>();
            foreach (var item in fileWords)
            {
                foreach (var word in item)
                {
                    if (!string.IsNullOrEmpty(word) && !fileWordsWithOutEmptyString.Contains(word))
                    {
                        fileWordsWithOutEmptyString.Add(word);
                    }
                }
            }

            return fileWordsWithOutEmptyString;
        }

        /// <summary>
        /// Verilen kelime listelerini kıyaslar, Birinci listede olup ikinci hashset'te olmayan kelimeleri, adetleri ile beraber dictionary olarak döner.
        /// </summary>
        /// <param name="firstFile"></param>
        /// <param name="secondFile"></param>
        /// <returns></returns>
        Dictionary<string, int> HangiKelimedenKacTaneVar(List<string> firstFile, HashSet<string> otherFiles)
        {
            Dictionary<string, int> wordsAndCount = new Dictionary<string, int>();
            foreach (var word in firstFile)
            {
                if (!otherFiles.Contains(word))
                {
                    if (wordsAndCount.ContainsKey(word))
                    {
                        wordsAndCount[word]++;
                    }
                    else
                    {
                        wordsAndCount.Add(word, 1);
                    }
                }
            }
            return wordsAndCount;
        }
        /// <summary>
        /// 1. metinde olupda diğer metinlerde olmayan kelimelerin, 1. dosyada kaç defa geçtiğini yazar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {

            string[] read1 = DosyaOku(filepath1);
            List<string[]> read2 = DosyalarOku(filepaths);

            List<string> firstFileWords = DosyaKelimeleriniListele(read1);
            HashSet<string> FilesWords = UniqueDosyaKelimeleriniListele(read2);

            Dictionary<string, int> wordsAndCount = HangiKelimedenKacTaneVar(firstFileWords, FilesWords);
            MetniYazdir(wordsAndCount);

        }



        /// <summary>
        /// metin kutusuna verilen dictionary'deki değerleri yazdırır
        /// </summary>
        /// <param name="wordsAndCount"></param>
        void MetniYazdir(Dictionary<string, int> wordsAndCount)
        {
            string result = string.Empty;

            foreach (var word in wordsAndCount)
            {
                result += $"<<{word.Key}>> diğer dosyalarda yoktur. <<{word.Value}>> kez birinci dosyada kullanılmaktadır. \r\n";
            }
            textBox3.Text = result;
        }
    }
}
