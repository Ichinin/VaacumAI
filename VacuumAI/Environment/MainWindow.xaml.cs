﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Environment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Contains the 15 squares of the game, labeled 0 to 14
        private List<Square> m_lGameSquare = new List<Square>();

        // Creation of a grid to act as the game set
        private Grid m_gEnvironnement = new Grid();
        private int nDust = 0;
        private int nJewel = 0;
        private static Timer aTimer = new Timer(10000);

        public MainWindow()
        {
            /*aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            aTimer.Interval = 1000;
            aTimer.Enabled = true;*/

            InitializeComponent();

            RowDefinition m_rRowGrid0 = new RowDefinition();
            RowDefinition m_rRowGrid1 = new RowDefinition();
            RowDefinition m_rRowGrid2 = new RowDefinition();
            m_gEnvironnement.RowDefinitions.Add(m_rRowGrid0);
            m_gEnvironnement.RowDefinitions.Add(m_rRowGrid1);
            m_gEnvironnement.RowDefinitions.Add(m_rRowGrid2);

            ColumnDefinition m_cColumnGrid0 = new ColumnDefinition();
            ColumnDefinition m_cColumnGrid1 = new ColumnDefinition();
            ColumnDefinition m_cColumnGrid2 = new ColumnDefinition();
            ColumnDefinition m_cColumnGrid3 = new ColumnDefinition();
            ColumnDefinition m_cColumnGrid4 = new ColumnDefinition();
            m_gEnvironnement.ColumnDefinitions.Add(m_cColumnGrid0);
            m_gEnvironnement.ColumnDefinitions.Add(m_cColumnGrid1);
            m_gEnvironnement.ColumnDefinitions.Add(m_cColumnGrid2);
            m_gEnvironnement.ColumnDefinitions.Add(m_cColumnGrid3);
            m_gEnvironnement.ColumnDefinitions.Add(m_cColumnGrid4);

            PopulateSquare();

            Content = m_gEnvironnement;

            RandomStuffs(2);
            MoveVacuum(0, 1);
        }

        /// <summary>
        /// Creates a list of 15 Squares, and address one to each cell of the environnement (0->14)
        /// </summary>
        private void PopulateSquare()
        {
            try
            {
                // Adds 15 Square instances in the list
                for (int i = 0; i < 15; i++)
                {
                    Square m_sGameSquare = new Square();
                    m_lGameSquare.Add(m_sGameSquare);
                }

                /* Creates a UserControl "Square" in each cell of the environnement
                 * We increment k to address a different index of m_lGameSquare to each cell */
                int k = 0;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        // Adds a Square in the cell (i,j)
                        Grid.SetRow(m_lGameSquare[k], i);
                        Grid.SetColumn(m_lGameSquare[k], j);

                        m_gEnvironnement.Children.Add(m_lGameSquare[k]);

                        m_lGameSquare[k].RemoveJewel();
                        m_lGameSquare[k].RemoveDust();
                        m_lGameSquare[k].RemoveVacuum();
                        // Remove everything from the 4 squares that aren't part of the environnement
                        if ((k == 3 || k == 4 || k == 13 || k == 14))
                        {
                            m_lGameSquare[k].BorderThickness = new Thickness(1000);
                        }
                        k++;
                    }
                }
                m_lGameSquare[0].AddVacuum();
            }
            catch (Exception ex)
            {
                Trace.Write(ex.Message);
            }
        }

        /// <summary>
        /// Moves the vacuum sprite from the case m_pCurrentLocation to m_pDestination
        /// </summary>
        /// <param name="m_pCurrentLocation">Case where the sprite will be removed</param>
        /// <param name="m_pDestination">Case where the sprite will be added</param>
        private void MoveVacuum(int m_pCurrentLocation, int m_pDestination)
        {
            try
            {
                if ((0 <= m_pCurrentLocation) && (m_pCurrentLocation <= 14) &&
                    (0 <= m_pDestination) && (m_pDestination <= 14)
                    )
                {
                    if (!((m_pDestination == 3) && (m_pDestination == 4) && (m_pDestination == 14) && (m_pDestination == 13)))
                    {
                        m_lGameSquare[m_pCurrentLocation].RemoveVacuum();
                        m_lGameSquare[m_pDestination].AddVacuum();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.Write(ex.Message);
            }
        }

        private void AddDust(int DustLocation)
        {
            m_lGameSquare[DustLocation].AddDust();
            nDust += 1;
        }

        private void AddJewel(int JewelLocation)
        {
            m_lGameSquare[JewelLocation].AddJewel();
            nJewel += 1;
        }

        private void RandomStuffs(int factors)
        {
            Random rand = new Random();
            int index = rand.Next(0, 13);

            if ((index != 3) || (index != 4))
            {
                if ((nDust / (nJewel + 1)) <= factors)
                {
                    // this.AddDust(index);
                }
                else
                {
                    this.AddJewel(index);
                }

            }
            else
            {
                index = rand.Next(0, 13);
            }
        }

    }
}
