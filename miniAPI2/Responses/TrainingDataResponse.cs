using Microsoft.ML;

namespace miniAPI2.Responses
{
    public class TrainingDataResponse
    {
        public string? DataId { get; set; }
        public string? Status{ get; set; }

        public List<MnistData> DataList { get; set; }


    }
}
