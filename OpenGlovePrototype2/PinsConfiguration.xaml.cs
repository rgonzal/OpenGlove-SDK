﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OpenGloveSDKConfigurationPrototype2;
using System.Windows.Forms;
using System.Xml.Linq;
using OpenGloveSDK;

namespace OpenGlovePrototype2
{
    /// <summary>
    /// Interaction logic for PinsConfiguration.xaml
    /// </summary>
    public partial class PinsConfiguration : Window
    {
        public class PinRow{
            public int Pin { get; set; }
            public String Polarity { get; set; }

            public PinRow(int pin)
            {
                this.Pin = pin;
            }
        }

        public class Board {
            public string name { get; set; }
            public List<int> pinNumbers { get; set; }

            public override string ToString()
            {
                return this.name;
            }
        }

        public List<PinRow> pins;

        List<String> Polarities;

        private List<Board> boards;

        private List<Board> openBoards() {
            List<Board> result = new List<Board>();

            XDocument xml = XDocument.Load("Boards.xml");
            List<XElement> xBoards = xml.Root.Elements("board").ToList();

            foreach (XElement xBoard in xBoards)
            {
                string name = xBoard.Element("name").Value;
                int pins = Int32.Parse(xBoard.Element("pins").Value);
                Board b = new Board();
                b.name = name;
                List<int> possiblePins = new List<int>();
                for (int i = 1; i <= pins; i++)
                {
                    possiblePins.Add(i);
                }
                b.pinNumbers = possiblePins;
                result.Add(b);
            }

            return result;
        }

        private void saveBoard(Board board) {
            XDocument xml = XDocument.Load("Boards.xml");

            XElement xboard = new XElement("board");
            XElement xname = new XElement("name");
            XElement xpins = new XElement("pins");

            xname.Value = board.name;
            xpins.Value = board.pinNumbers.Count.ToString();

            xboard.Add(xname);
            xboard.Add(xpins);

            xml.Root.Add(xboard);

            xml.Save("Boards.xml");

            this.boards.Add(board);
        }

        private void initializeBoards() {
            this.boards = openBoards();
            this.comboBoxBoard.SelectedIndex = 0;
            this.initializePinsList(0);
            this.comboBoxBoard.SelectionChanged += this.comboBoxBoard_SelectionChanged;
        }

        public PinsConfiguration()
        {
            
            InitializeComponent();

            initializeBoards();

            
        }

        private void initializePinsList(int boardIndex) {
            this.pins = new List<PinRow>();
            foreach (int pin in this.boards[boardIndex].pinNumbers)
            {
                this.pins.Add(new PinRow(pin));
            }
            this.dataGridPins.ItemsSource = this.pins;

            Polarities = new List<string>() { "Positive", "Negative" };

            Polarity.ItemsSource = Polarities;

            this.comboBoxBaudRate.ItemsSource = OGCore.GetCore().gloveCfg.allowedBaudRates;
            this.comboBoxBoard.ItemsSource = this.boards;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            OGCore core = OGCore.GetCore();

            SaveFileDialog saveConfigurationDialog = new SaveFileDialog();
            saveConfigurationDialog.Filter = "XML-File | *.xml";
            saveConfigurationDialog.Title = "Save your configuration file";
            saveConfigurationDialog.ShowDialog();

            if (saveConfigurationDialog.FileName != "") {
                if (this.comboBoxBaudRate.SelectedItem != null)
                {
                    core.gloveCfg.BaudRate = Int32.Parse(this.comboBoxBaudRate.SelectedItem.ToString());
                    core.gloveCfg.positivePins = new List<int>();
                    core.gloveCfg.negativePins = new List<int>();

                    foreach (PinRow pin in pins)
                    {
                        if (pin.Polarity != null)
                        {
                            if (pin.Polarity.Equals("Positive"))
                            {
                                core.gloveCfg.positivePins.Add(pin.Pin);
                            }
                            else
                            {
                                core.gloveCfg.negativePins.Add(pin.Pin);
                            }
                        }
                    }
                    /*
                    ConfigurationTool mw = new ConfigurationTool();

                    mw.Show();

                    this.Close();*/
                    core.gloveCfg.saveGloveConfiguration(saveConfigurationDialog.FileName);
                }
                else
                {
                    string message = "Must select BaudRate";
                    string caption = "BaudRate";
                    MessageBoxButton button = MessageBoxButton.OK;
                    System.Windows.MessageBox.Show(message, caption, button, MessageBoxImage.Error);
                }
            }
 
        }

        private void buttonAddBoard_Click(object sender, RoutedEventArgs e)
        {
            AddBoard addBoard = new AddBoard();
            addBoard.ShowDialog();

            List<int> pins = new List<int>();

            if (addBoard.BoardName != null) {
                if (! addBoard.BoardName.Equals("")) {
                    Board b = new Board();
                    b.name = addBoard.BoardName;
                    for (int i = 1; i <= addBoard.Pins; i++)
                    {
                        pins.Add(i);
                    }
                    b.pinNumbers = pins;

                    this.saveBoard(b);
                }
            }
        }

        private void comboBoxBoard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           this.initializePinsList(((System.Windows.Controls.ComboBox) sender).SelectedIndex);
        }
    }
}
