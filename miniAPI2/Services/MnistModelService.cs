using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using Microsoft.ML.Data;
using miniAPI2.Responses;
using System;
using System.Collections.Generic;
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

    public readonly Dictionary<string, IDataView> _dataViewStorage = new Dictionary<string, IDataView>();

    public TrainingDataResponse LoadTrainData(string trainDataPath)
    {
        if (!string.IsNullOrEmpty(trainDataPath))
        {
            var trainData = _mlContext.Data.LoadFromTextFile<MnistData>(trainDataPath, hasHeader: true, separatorChar: ',');
            var enumerableData = _mlContext.Data.CreateEnumerable<MnistData>(trainData, reuseRowObject: false).Take(100);

            // Generate a unique identifier
            var dataId = Guid.NewGuid().ToString();

            // Store the IDataView in memory
            _dataViewStorage[dataId] = trainData;

            return new TrainingDataResponse()
            {
                DataId = dataId,
                Status = "Load Training Data complete.",
                DataList = enumerableData.ToList()
            };
        }
        else
        {
            return new TrainingDataResponse()
            {
                DataId = null,
                Status = "Error on Loading data.",
                DataList = null
            };
        }
    }

    public string TrainModel(string dataId)
    {
        if (!string.IsNullOrEmpty(dataId) && _dataViewStorage.ContainsKey(dataId))
        {
            var trainData = _dataViewStorage[dataId];

            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey("Label")
                .Append(_mlContext.Transforms.Concatenate("Features", nameof(MnistData.Pixels)))
                .Append(_mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(_mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy("Label", "Features"))
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            _model = pipeline.Fit(trainData);

            // Save the trained model
            _mlContext.Model.Save(_model, trainData.Schema, "mnist_model.zip");
            return "Model training complete.";
        }
        else
        {
            return "No data to Train on!";
        }
    }
    // sample : 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,255,255,255,255,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,255,255,255,255,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,255,255,255,255,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,255,255,255,255,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,255,255,255,255,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,255,255,255,255,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
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
