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

    public double[] CalculateGradients(float[] pixels, float[] predictedValues, float[] actualValues)
    {
        // Assuming these are class-level or passed as arguments
        double[] weights = new double[pixels.Length]; // model weights for each pixel
        double bias = 0; // single bias value for simplicity in this example
        double learningRate = 0.01; // learning rate

        // Initialize gradients
        double[] dLoss_dWeights = new double[weights.Length]; // gradients of loss w.r.t. weights
        double dLoss_dBias = 0; // gradient of loss w.r.t. bias

        // Calculate the gradient of the loss w.r.t. the output (dL/dO)
        // Assuming Mean Squared Error (MSE) Loss function
        double[] dLoss_dOutputs = new double[predictedValues.Length];
        for (int i = 0; i < predictedValues.Length; i++)
        {
            dLoss_dOutputs[i] = 2 * (predictedValues[i] - actualValues[i]) / predictedValues.Length;
        }

        // Calculate the gradient of the loss w.r.t. the weights (dL/dW) and bias
        for (int i = 0; i < weights.Length; i++)
        {
            dLoss_dWeights[i] = dLoss_dOutputs[0] * pixels[i]; // Assuming one output
            weights[i] -= learningRate * dLoss_dWeights[i]; // Update weights
        }

        dLoss_dBias = dLoss_dOutputs[0];
        bias -= learningRate * dLoss_dBias; // Update bias

        // Return the gradients (you may not need to return them depending on how you're updating the weights)
        return dLoss_dWeights;
    }
    public float[] UpdateWeights(float[] weights, float[] gradients, float learningRate)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            // Update weight based on gradient and learning rate
            weights[i] -= learningRate * gradients[i];
        }
        //_mlContext.Model.Save(_model, trainData.Schema, "mnist_model.zip");


        return weights;
    }

}
