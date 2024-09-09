using Microsoft.ML.Data;

public class MnistData
{
    [LoadColumn(0)]
    public float Label { get; set; }

    [LoadColumn(1, 784)]
    [VectorType(784)]
    public float[] Pixels { get; set; }
}

public class MnistPrediction
{
    [ColumnName("PredictedLabel")]
    public float PredictedLabel { get; set; }
}



