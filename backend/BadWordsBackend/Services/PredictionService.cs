using Microsoft.ML;
using System.Data.SQLite;

namespace BadWordsBackend.Services
{
    public class CommentData
    {
        public string Text { get; set; } = "";
        public float Label { get; set; }
    }

    public class CommentPrediction
    {
        public float Score { get; set; }
    }

    public class PredictionService
    {
        private readonly string _dbPath = "Data/badwords_train.db";
        private readonly string _modelPath = "Model/modelo.zip";
        private readonly MLContext _mlContext;
        private PredictionEngine<CommentData, CommentPrediction> _predictor;

        public PredictionService()
        {
            _mlContext = new MLContext();
            TrainModel();
        }

        private void TrainModel()
        {
            var data = LoadDataFromSQLite();
            var trainingData = _mlContext.Data.LoadFromEnumerable(data);

            var pipeline = _mlContext.Transforms.Text.FeaturizeText("Features", nameof(CommentData.Text))
                .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: "Label"));

            var model = pipeline.Fit(trainingData);

            _mlContext.Model.Save(model, trainingData.Schema, _modelPath);
            _predictor = _mlContext.Model.CreatePredictionEngine<CommentData, CommentPrediction>(model);
        }

        private List<CommentData> LoadDataFromSQLite()
        {
            var comments = new List<CommentData>();
            using var conn = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
            conn.Open();
            var cmd = new SQLiteCommand("SELECT Text, Label FROM Comments", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                comments.Add(new CommentData
                {
                    Text = reader.GetString(0),
                    Label = reader.GetInt32(1)
                });
            }
            return comments;
        }

        public string Predict(string text)
        {
            var prediction = _predictor.Predict(new CommentData { Text = text });
            return prediction.Score > 0.5 ? "Malas palabras" : "Limpio";
        }
    }
}
