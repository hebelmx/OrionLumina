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
    /// https://pytorch.org/tutorials/beginner/transformer_tutorial.html
    ///
    /// It relies on the WikiText2 dataset, which can be downloaded at:
    ///
    /// https://s3.amazonaws.com/research.metamind.io/wikitext/wikitext-2-v1.zip
    ///
    /// After downloading, extract the files using the defaults (Windows only).
    /// </summary>
    public class SequenceToSequence
    {
        // This path assumes that you're running this on Windows.
        private static readonly string DataLocation = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "..", "Downloads", "wikitext-2-v1");

        private const long Emsize = 200;
        private const long Nhid = 200;
        private const long Nlayers = 2;
        private const long Nhead = 2;
        private const double Dropout = 0.2;

        private const int BatchSize = 64;
        private const int EvalBatchSize = 32;

        internal static void Run(int epochs, int timeout, string logdir)

        {
            torch.random.manual_seed(1);

            var cwd = Environment.CurrentDirectory;

            var device =
                torch.cuda.is_available() ? torch.CUDA :
                torch.mps_is_available() ? torch.MPS :
                torch.CPU;

            Console.WriteLine();
            Console.WriteLine($"\tRunning SequenceToSequence on {device.type.ToString()} for {epochs} epochs, terminating after {TimeSpan.FromSeconds(timeout)}.");
            Console.WriteLine();

            Console.WriteLine($"\tPreparing training and test data...");

            var vocabIter = TorchText.Datasets.WikiText2("train", DataLocation);
            var tokenizer = TorchText.Data.Utils.get_tokenizer("basic_english");

            var counter = new TorchText.Vocab.Counter<string>();
            foreach (var item in vocabIter)
            {
                counter.update(tokenizer(item));
            }

            var vocab = new TorchText.Vocab.Vocab(counter);

            var (trainIter, validIter, testIter) = TorchText.Datasets.WikiText2(DataLocation);

            var trainData = Batchify(ProcessInput(trainIter, tokenizer, vocab), BatchSize).to((Device)device);
            var validData = Batchify(ProcessInput(validIter, tokenizer, vocab), EvalBatchSize).to((Device)device);
            var testData = Batchify(ProcessInput(testIter, tokenizer, vocab), EvalBatchSize).to((Device)device);

            var bptt = 32;

            var ntokens = vocab.Count;

            Console.WriteLine($"\tCreating the model...");
            Console.WriteLine();

            var model = new TransformerModel(ntokens, Emsize, Nhead, Nhid, Nlayers, Dropout).To((Device)device);
            var loss = CrossEntropyLoss();
            var lr = 2.50;
            var optimizer = torch.optim.SGD(model.parameters(), lr);
            var scheduler = torch.optim.lr_scheduler.StepLR(optimizer, 1, 0.95, last_epoch: 15);

            var writer = String.IsNullOrEmpty(logdir) ? null : torch.utils.tensorboard.SummaryWriter(logdir, createRunName: true);

            var totalTime = new Stopwatch();
            totalTime.Start();

            foreach (var epoch in Enumerable.Range(1, epochs))
            {

                var sw = new Stopwatch();
                sw.Start();

                Train(epoch, trainData, model, loss, bptt, ntokens, optimizer);

                var valLoss = Evaluate(validData, model, loss, bptt, ntokens, optimizer);
                sw.Stop();

                Console.WriteLine($"\nEnd of epoch: {epoch} | lr: {optimizer.ParamGroups.First().LearningRate:0.00} | time: {sw.Elapsed.TotalSeconds:0.0}s | loss: {valLoss:0.00}\n");
                scheduler.step();

                if (writer != null)
                {
                    writer.add_scalar("seq2seq/loss", (float)valLoss, epoch);
                }

                if (totalTime.Elapsed.TotalSeconds > timeout) break;
            }

            var tstLoss = Evaluate(testData, model, loss, bptt, ntokens, optimizer);
            totalTime.Stop();

            Console.WriteLine($"\nEnd of training | time: {totalTime.Elapsed.TotalSeconds:0.0}s | loss: {tstLoss:0.00}\n");
        }

        private static void Train(int epoch, Tensor trainData, TransformerModel model, Loss<Tensor, Tensor, Tensor> criterion, int bptt, int ntokens, torch.optim.Optimizer optimizer)
        {
            model.train();

            var totalLoss = 0.0f;

            using var d = torch.NewDisposeScope();
            var batch = 0;
            var logInterval = 200;

            var srcMask = model.GenerateSquareSubsequentMask(bptt);

            var tdlen = trainData.shape[0];


            for (var i = 0; i < tdlen - 1; batch++, i += bptt)
            {

                var (data, targets) = GetBatch(trainData, i, bptt);
                optimizer.zero_grad();

                if (data.shape[0] != bptt)
                {
                    srcMask = model.GenerateSquareSubsequentMask(data.shape[0]);
                }

                using (var output = model.forward(data, srcMask))
                {
                    var loss = criterion.forward(output.view(-1, ntokens), targets);
                    loss.backward();
                    torch.nn.utils.clip_grad_norm_(model.parameters(), 0.5);
                    optimizer.step();

                    totalLoss += loss.to(torch.CPU).item<float>();
                }

                if (batch % logInterval == 0 && batch > 0)
                {
                    var curLoss = totalLoss / logInterval;
                    Console.WriteLine($"epoch: {epoch} | batch: {batch} / {tdlen / bptt} | loss: {curLoss:0.00}");
                    totalLoss = 0;
                }

                d.DisposeEverythingBut(srcMask);
            }
        }

        private static double Evaluate(Tensor evalData, TransformerModel model, Loss<Tensor, Tensor, Tensor> criterion, int bptt, int ntokens, torch.optim.Optimizer optimizer)
        {
            model.eval();

            using var d = torch.NewDisposeScope();
            var srcMask = model.GenerateSquareSubsequentMask(bptt);

            var totalLoss = 0.0f;
            var batch = 0;


            for (var i = 0; i < evalData.shape[0] - 1; batch++, i += bptt)
            {

                var (data, targets) = GetBatch(evalData, i, bptt);
                if (data.shape[0] != bptt)
                {
                    srcMask = model.GenerateSquareSubsequentMask(data.shape[0]);
                }
                using (var output = model.forward(data, srcMask))
                {
                    var loss = criterion.forward(output.view(-1, ntokens), targets);
                    totalLoss += data.shape[0] * loss.to(torch.CPU).item<float>();
                }

                data.Dispose();
                targets.Dispose();

                d.DisposeEverythingBut(srcMask);
            }

            return totalLoss / evalData.shape[0];
        }

        static Tensor ProcessInput(IEnumerable<string> iter, Func<string, IEnumerable<string>> tokenizer, TorchText.Vocab.Vocab vocab)
        {
            var data = new List<Tensor>();
            foreach (var item in iter)
            {
                var itemData = new List<long>();
                foreach (var token in tokenizer(item))
                {
                    itemData.Add(vocab[token]);
                }
                data.Add(torch.tensor(itemData.ToArray(), torch.int64));
            }

            var result = torch.cat(data.Where(t => t.NumberOfElements > 0).ToList(), 0);
            return result;
        }

        static Tensor Batchify(Tensor data, int batchSize)
        {
            var nbatch = data.shape[0] / batchSize;
            using var d2 = data.narrow(0, 0, nbatch * batchSize).view(batchSize, -1).t();
            return d2.contiguous();
        }

        static (Tensor, Tensor) GetBatch(Tensor source, int index, int bptt)
        {
            var len = Math.Min(bptt, source.shape[0] - 1 - index);
            var data = source[TensorIndex.Slice(index, index + len)];
            var target = source[TensorIndex.Slice(index + 1, index + 1 + len)].reshape(-1);
            return (data, target);
        }

    }
}
