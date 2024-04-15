using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace HeartDisease.Models
{
    public class class_bayes
    {

        public string classification(value Atrr_value)
        {
            Console.WriteLine("\n Bayes \n");

            string fn = " E:\\online\\datamining\\heart_disease_male.txt";


            int nx = 7;  // Number predictor variables
            int nc = 2;  // Number classes
            int N = 209;  // Number data items

            string[][] data = LoadData(fn, N, nx + 1, ',');
            Console.WriteLine("Training data:");
            for (int i = 0; i < 5; ++i)
            {
                Console.Write("[" + i + "] ");
                for (int j = 0; j < nx + 1; ++j)
                {
                    Console.Write(data[i][j] + " ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine(". . . \n");
            int[][] jointCts = MatrixInt(nx, nc);
            int[] yCts = new int[nc];

            //string[] X = new string[] { "43", "asympt", "140", "FALSE", "normal", "135", "yes" };
            string[] Arr_value = new string[] { Atrr_value.age.ToString(), Atrr_value.chest_pain_type, Atrr_value.rest_blood_pressure.ToString(), Atrr_value.blood_sugar, Atrr_value.rest_electro, Atrr_value.max_heart_rate.ToString(), Atrr_value.exercice_angina };



            Console.WriteLine("Item to classify: ");
            for (int i = 0; i < nx; ++i)
                Console.Write(Arr_value[i] + " ");
            Console.WriteLine("\n");
            // Compute joint counts and y counts
            for (int i = 0; i < N; ++i)
            {
                int y = int.Parse(data[i][nx]);
                ++yCts[y];
                for (int j = 0; j < nx; ++j)
                {
                    if (data[i][j] == Arr_value[j])
                        ++jointCts[j][y];
                }
            }
            // Laplacian smoothing
            for (int i = 0; i < nx; ++i)
                for (int j = 0; j < nc; ++j)
                    ++jointCts[i][j];
            Console.WriteLine("Joint counts: ");
            for (int i = 0; i < nx; ++i)
            {
                for (int j = 0; j < nc; ++j)
                {
                    Console.Write(jointCts[i][j] + " ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("\nClass counts: ");
            for (int k = 0; k < nc; ++k)
                Console.Write(yCts[k] + " ");
            Console.WriteLine("\n");
            // Compute evidence terms
            double[] eTerms = new double[nc];
            for (int k = 0; k < nc; ++k)
            {
                double v = 1.0;
                for (int j = 0; j < nx; ++j)
                {
                    v *= (double)(jointCts[j][k]) / (yCts[k] + nx);
                }
                v *= (double)(yCts[k]) / N;
                eTerms[k] = v;
            }

            Console.WriteLine("Evidence terms:");
            for (int k = 0; k < nc; ++k)
                Console.Write(eTerms[k].ToString("F4") + " ");
            Console.WriteLine("\n");

            double evidence = 0.0;
            for (int k = 0; k < nc; ++k)
                evidence += eTerms[k];
            double[] probs = new double[nc];
            for (int k = 0; k < nc; ++k)
                probs[k] = eTerms[k] / evidence;

            Console.WriteLine("Probabilities: ");
            for (int k = 0; k < nc; ++k)
                Console.Write(probs[k].ToString("F4") + " ");
            Console.WriteLine("\n");
            int pc = ArgMax(probs);
            Id3 id = new Id3();
            Atrr_value = id.classification(Atrr_value);
            Console.WriteLine("Predicted class: ");
            Console.WriteLine(pc);
            Console.WriteLine("\nEnd naive Bayes ");
            string result = Atrr_value.result;
           
            return result;
        }
        static string[][] MatrixString(int rows, int cols)
        {
            string[][] result = new string[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new string[cols];
            return result;
        }
        static int[][] MatrixInt(int rows, int cols)
        {
            int[][] result = new int[rows][];
            for (int i = 0; i < rows; ++i)
                result[i] = new int[cols];
            return result;
        }
        static string[][] LoadData(string fn, int rows,
          int cols, char delimit)
        {
            string[][] result = MatrixString(rows, cols);
            FileStream ifs = new FileStream(fn, FileMode.Open);
            StreamReader sr = new StreamReader(ifs);
            string[] tokens = null;
            string line = null;
            int i = 0;
            while ((line = sr.ReadLine()) != null)
            {
                tokens = line.Split(delimit);
                for (int j = 0; j < cols; ++j)
                    result[i][j] = tokens[j];
                ++i;
            }
            sr.Close(); ifs.Close();
            return result;
        }
        static int ArgMax(double[] vector)
        {
            int result = 0;
            double maxV = vector[0];
            for (int i = 0; i < vector.Length; ++i)
            {
                if (vector[i] > maxV)
                {
                    maxV = vector[i];
                    result = i;
                }
            }
            return result;
        }

    }
}