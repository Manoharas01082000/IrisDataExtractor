using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IrisDataExtractor
{
    public partial class MainPage : ContentPage
    {
        private string selectedFilePath;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnSelectFileClicked(object sender, EventArgs e)
        {
            try
            {
                var fileResult = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileTypeHelpers.GetMimeTypes(".csv", ".xlsx"),
                    PickerTitle = "Select File"
                });

                if (fileResult != null)
                {
                    selectedFilePath = fileResult.FullPath;
                    SelectedFileLabel.Text = $"Selected File: {Path.GetFileName(selectedFilePath)}";
                }
            }
            catch (Exception ex)
            {
                SelectedFileLabel.Text = $"Error selecting file: {ex.Message}";
            }
        }

        private void OnExtractClicked(object sender, EventArgs e)
        {
            double sepalLength, sepalWidth, petalLength, petalWidth;

            // Check if the user input is valid
            if (double.TryParse(SepalLengthEntry.Text, out sepalLength) &&
                double.TryParse(SepalWidthEntry.Text, out sepalWidth) &&
                double.TryParse(PetalLengthEntry.Text, out petalLength) &&
                double.TryParse(PetalWidthEntry.Text, out petalWidth))
            {
                // Extract iris data based on the provided values and selected file
                string extractedData = ExtractIrisData(selectedFilePath, sepalLength, sepalWidth, petalLength, petalWidth);

                ResultLabel.Text = extractedData;
            }
            else
            {
                ResultLabel.Text = "Invalid input. Please enter numeric values.";
            }
        }

        private string ExtractIrisData(string filePath, double sepalLength, double sepalWidth, double petalLength, double petalWidth)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return "Please select a valid file.";
            }

            // Read the data from the selected file
            var data = ReadDataFromFile(filePath);

            // Filter the data based on the provided values
            var filteredData = data.Where(d =>
                d.SepalLength == sepalLength &&
                d.SepalWidth == sepalWidth &&
                d.PetalLength == petalLength &&
                d.PetalWidth == petalWidth);
            string extractedData = $"Extracted iris data based on the provided values from {Path.GetFileName(filePath)}:\n";

            if (filteredData.Any())
            {
                foreach (var item in filteredData)
                {
                    extractedData += $"Sepal Length: {item.SepalLength}, Sepal Width: {item.SepalWidth}, " +
                        $"Petal Length: {item.PetalLength}, Petal Width: {item.PetalWidth}\n";
                }
            }
            else
            {
                extractedData += "No matching data found.";
            }

            return extractedData;
        }

        private IrisData[] ReadDataFromFile(string filePath)
        {
            // Read the data from the file (assuming CSV format or Excel)
            // Implement the appropriate logic based on the file type (CSV or Excel)
            if (Path.GetExtension(filePath) == ".csv")
            {
                var csvLines = File.ReadAllLines(filePath);
                return csvLines.Skip(1) // Skip header row
                               .Select(line => line.Split(','))
                               .Select(parts => new IrisData
                               {
                                   SepalLength = double.Parse(parts[0]),
                                   SepalWidth = double.Parse(parts[1]),
                                   PetalLength = double.Parse(parts[2]),
                                   PetalWidth = double.Parse(parts[3])
                               })
                               .ToArray();
            }
            else if (Path.GetExtension(filePath) == ".xlsx")
            {
                // Add logic to read Excel file
                // You can use libraries like EPPlus, ClosedXML, or the Microsoft Excel Interop
            }

            return new IrisData[0];
        }
    }

    public class IrisData
    {
        public double SepalLength { get; set; }
        public double SepalWidth { get; set; }
        public double PetalLength { get; set; }
        public double PetalWidth { get; set; }
    }
}