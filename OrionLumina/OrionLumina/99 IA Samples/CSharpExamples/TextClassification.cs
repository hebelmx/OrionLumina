// Copyright (c) .NET Foundation and Contributors.  All Rights Reserved.  See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Models;
using TorchSharp;
using TorchSharp.Examples;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;

namespace CSharpExamples
{
    /// <summary>
    /// This example is based on the PyTorch tutorial at:
    /// 
    /// https://pytorch.org/tutorials/beginner/text_sentiment_ngrams_tutorial.html
    ///
    /// It relies on the AG_NEWS dataset, which can be downloaded in CSV form at:
    ///
    /// https://github.com/mhjabreel/CharCnn_Keras/tree/master/data/ag_news_csv
    ///
    /// Download the two files, and place them in a folder called "AG_NEWS" in
    /// accordance with the file path below (Windows only).
    ///
    /// </summary>
    public class TextClassification
    {
        private const long Emsize = 200;

        private const long BatchSize = 128;
        private const long EvalBatchSize = 128;

        private const int Epochs = 15;

        // This path assumes that you're running this on Windows.
        private static readonly string DataLocation = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "..", "Downloads", "AG_NEWS");

        internal static void Run(int epochs, int timeout, string logdir)
        {
            torch.random.manual_seed(1);

            var cwd = Environment.CurrentDirectory;

            var device =
                torch.cuda.is_available() ? torch.CUDA :
                torch.mps_is_available() ? torch.MPS :
                torch.CPU;

            Console.WriteLine();
            Console.WriteLine($"\tRunning TextClassification on {device.type.ToString()} for {epochs} epochs, terminating after {TimeSpan.FromSeconds(timeout)}.");
            Console.WriteLine();

            Console.WriteLine($"\tPreparing training and test data...");

            using var reader = TorchText.Data.AG_NEWSReader.AG_NEWS("train", (Device)device, DataLocation);
            var dataloader = reader.Enumerate();

            var tokenizer = TorchText.Data.Utils.get_tokenizer("basic_english");

            var counter = new TorchText.Vocab.Counter<string>();
            foreach (var (label, text) in dataloader)
            {
                counter.update(tokenizer(text));
            }

            var vocab = new TorchText.Vocab.Vocab(counter);


            Console.WriteLine($"\tCreating the model...");
            Console.WriteLine();

            var model = new TextClassificationModel(vocab.Count, Emsize, 4).to((Device)device);

            var loss = CrossEntropyLoss();
            var lr = 5.0;
            var optimizer = torch.optim.SGD(model.parameters(), lr);
            var scheduler = torch.optim.lr_scheduler.StepLR(optimizer, 1, 0.2, last_epoch: 5);

            var totalTime = new Stopwatch();
            totalTime.Start();

            foreach (var epoch in Enumerable.Range(1, epochs))
            {

                var sw = new Stopwatch();
                sw.Start();

                Train(epoch, reader.GetBatches(tokenizer, vocab, BatchSize), model, loss, optimizer);

                sw.Stop();

                Console.WriteLine($"\nEnd of epoch: {epoch} | lr: {optimizer.ParamGroups.First().LearningRate:0.0000} | time: {sw.Elapsed.TotalSeconds:0.0}s\n");
                scheduler.step();

                if (totalTime.Elapsed.TotalSeconds > timeout) break;
            }

            totalTime.Stop();

            using var testReader = TorchText.Data.AG_NEWSReader.AG_NEWS("test", (Device)device, DataLocation);
            {

                var sw = new Stopwatch();
                sw.Start();

                var accuracy = Evaluate(testReader.GetBatches(tokenizer, vocab, EvalBatchSize), model, loss);

                sw.Stop();

                Console.WriteLine($"\nEnd of training: test accuracy: {accuracy:0.00} | eval time: {sw.Elapsed.TotalSeconds:0.0}s\n");
                scheduler.step();
            }
        }

        static void Train(int epoch, IEnumerable<(Tensor, Tensor, Tensor)> trainData, TextClassificationModel model, Loss<Tensor, Tensor, Tensor> criterion, torch.optim.Optimizer optimizer)
        {
            model.train();

            var totalAcc = 0.0;
            long totalCount = 0;
            long logInterval = 250;

            var batch = 0;

            var batchCount = trainData.Count();

            using var d = torch.NewDisposeScope();
            foreach (var (labels, texts, offsets) in trainData)
            {

                optimizer.zero_grad();

                using (var predictedLabels = model.forward(texts, offsets))
                {

                    var loss = criterion.forward(predictedLabels, labels);
                    loss.backward();
                    torch.nn.utils.clip_grad_norm_(model.parameters(), 0.5);
                    optimizer.step();

                    totalAcc += (predictedLabels.argmax(1) == labels).sum().to(torch.CPU).item<long>();
                    totalCount += labels.size(0);
                }

                if (batch % logInterval == 0 && batch > 0)
                {
                    var accuracy = totalAcc / totalCount;
                    Console.WriteLine($"epoch: {epoch} | batch: {batch} / {batchCount} | accuracy: {accuracy:0.00}");
                }
                batch += 1;
            }
        }

        static double Evaluate(IEnumerable<(Tensor, Tensor, Tensor)> testData, TextClassificationModel model, Loss<Tensor, Tensor, Tensor> criterion)
        {
            model.eval();

            var totalAcc = 0.0;
            long totalCount = 0;

            using var d = torch.NewDisposeScope();
            foreach (var (labels, texts, offsets) in testData)
            {
                using var predictedLabels = model.forward(texts, offsets);
                var loss = criterion.forward(predictedLabels, labels);

                totalAcc += (predictedLabels.argmax(1) == labels).sum().to(torch.CPU).item<long>();
                totalCount += labels.size(0);
            }

            return totalAcc / totalCount;
        }
    }
}
