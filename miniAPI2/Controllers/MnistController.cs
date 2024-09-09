using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
namespace miniAPI2.Controllers;

[ApiController]
[Route("[controller]")]

public class MnistController : ControllerBase
{
    private readonly MnistModelService _mnistModelService;
    public string dataId = "";

    public MnistController(MnistModelService mnistModelService)
    {
        _mnistModelService = mnistModelService;
    }

    [HttpPost("LoadTrainData")]
    public IActionResult LoadTrainData([FromBody] string trainDataPath)
    {
        var response = _mnistModelService.LoadTrainData(trainDataPath);
        return Ok(response);
    }

    [HttpPost("train")]
    public IActionResult TrainModel()
    {
        var dataId = _mnistModelService._dataViewStorage.FirstOrDefault();
        var ret = _mnistModelService.TrainModel(dataId.Key);
        return Ok(ret);
    }

    [HttpPost("predict")]
    public IActionResult Predict([FromBody] float[] pixels)
    {
        var predictedLabel = _mnistModelService.Predict(pixels);
        return Ok(predictedLabel);
    }

}
