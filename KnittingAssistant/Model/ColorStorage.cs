﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace KnittingAssistant.Model
{
    public class ColorStorage
    {
        private string filename;
        private static ColorStorage instance;

        public LinkedList<Color> ColorList { get; }
        public int ColorsCount { get => ColorList.Count; }        
        public Dictionary<Color, int> ColorsCounting { get; }
        public int CountingColorsCount { get => ColorsCounting.Count; }

        public ColorStorage(string filename)
        {
            this.filename = filename;
            ColorList = new LinkedList<Color>();
            ColorsCounting = new Dictionary<Color, int>();
        }

        public ColorStorage() : this("colors.txt") { }

        public static ColorStorage GetInstance()
        {
            if (instance == null)
                instance = new ColorStorage();
            return instance;
        }

        public void ReadColorsFromFile()
        {
            ClearColors();
            if (File.Exists(filename))
            {
                string[] storageContent = File.ReadAllLines(filename);
                byte[,] colorBytes = new byte[storageContent.Length, 3];

                for (int i = 0; i < storageContent.Length; i++)
                {
                    var colorString = storageContent[i].Split(" ");
                    colorBytes[i, 0] = byte.Parse(colorString[0]);
                    colorBytes[i, 1] = byte.Parse(colorString[1]);
                    colorBytes[i, 2] = byte.Parse(colorString[2]);

                    ColorList.AddLast(Color.FromRgb(colorBytes[i, 0], colorBytes[i, 1], colorBytes[i, 2]));
                }
            }
        }

        public void WriteColorsToFile()
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(filename);

                foreach (Color color in ColorList)
                {
                    string writeableColor = color.R.ToString() + " " + color.G.ToString() + " " + color.B.ToString();
                    sw.WriteLine(writeableColor);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
        }

        public void AddColor(Color color)
        {
            if (!ColorList.Contains(color))
                ColorList.AddLast(color);
        }

        public void RefreshColorsCounting()
        {
            ClearColorsCounting();
            foreach (var color in ColorList)
                ColorsCounting.Add(color, 0);
        }

        public void RemoveColor(int index) => ColorList.Remove(ColorList.ElementAt(index));
        public void RemoveColor(LinkedListNode<Color> colorNode) => ColorList.Remove(colorNode);
        public void ClearColors()
        {
            ColorList.Clear();
        }
        public void ClearColorsCounting() => ColorsCounting.Clear();
        public LinkedListNode<Color> GetNodeByIndex(int index) => ColorList.Find(ColorList.ElementAt(index));
    }
}