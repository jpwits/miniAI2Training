static class Init
{
    static Init()
    {
        // Path to the MNIST CSV file
        string filePath = "C:\\Users\\JosuaW\\source\\repos\\miniAIConsole\\miniAIConsole\\mnist_test.csv";

        // Load data from CSV file
        List<(int label, float[] pixels)> mnistData = LoadMnistData(filePath);

        // Example: Print the first label and pixel data
        var firstExample = mnistData[0];
        Console.WriteLine("Label: " + firstExample.label);
        Console.WriteLine("Pixel Data: " + string.Join(", ", firstExample.pixels));
    }

    static List<(int label, float[] pixels)> LoadMnistData(string filePath)
    {
        var data = new List<(int label, float[] pixels)>();

        using (var reader = new StreamReader(filePath))
        {
            // Skip the header line
            reader.ReadLine();

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var values = line.Split(',');

                // The first value is the label (digit)
                int label = int.Parse(values[0]);

                // The remaining values are the pixel data
                float[] pixels = new float[values.Length - 1];
                for (int i = 1; i < values.Length; i++)
                {
                    pixels[i - 1] = float.Parse(values[i]); // No normalization as the values seem to be 0
                }

                data.Add((label, pixels));
            }
        }

        return data;
    }
}

