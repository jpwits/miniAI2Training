using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Data;
using System.IO;
using System.Linq;

public class MnistModelService
{
    private readonly MLContext _mlContext;
    private ITransformer _model;

    public MnistModelService()
    {
        _mlContext = new MLContext();
    }

    public IDataView TrainModel(string trainDataPath)
    {
        IDataView trainData = _mlContext.Data.LoadFromTextFile<MnistData>(trainDataPath, hasHeader: true, separatorChar: ',');

        var pipeline = _mlContext.Transforms.Conversion.MapValueToKey("Label")
            .Append(_mlContext.Transforms.Concatenate("Features", nameof(MnistData.Pixels)))
            .Append(_mlContext.Transforms.NormalizeMinMax("Features"))
            .Append(_mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy("Label", "Features"))
            .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        _model = pipeline.Fit(trainData);

        // Save the trained model
        _mlContext.Model.Save(_model, trainData.Schema, "mnist_model.zip");
    }

    public float Predict(float[] pixels)
    {
        if (_model == null)
        {
            // Load the model if it hasn't been loaded yet
            using var stream = new FileStream("mnist_model.zip", FileMode.Open, FileAccess.Read, FileShare.Read);
            _model = _mlContext.Model.Load(stream, out var _);
        }

        var predictionEngine = _mlContext.Model.CreatePredictionEngine<MnistData, MnistPrediction>(_model);

        var input = new MnistData { Pixels = pixels };
        var prediction = predictionEngine.Predict(input);

        return prediction.PredictedLabel;
    }
}
