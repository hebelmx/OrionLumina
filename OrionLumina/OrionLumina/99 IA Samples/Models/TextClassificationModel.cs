// Copyright (c) .NET Foundation and Contributors.  All Rights Reserved.  See LICENSE in the project root for license information.

using static TorchSharp.torch;
using static TorchSharp.torch.nn;

namespace Models
{
    /// <summary>
    /// This example is based on the PyTorch tutorial at:
    /// 
    /// https://pytorch.org/tutorials/beginner/text_sentiment_ngrams_tutorial.html
    ///
    /// </summary>
    public class TextClassificationModel : Module<Tensor, Tensor, Tensor>
    {
        private TorchSharp.Modules.EmbeddingBag _embedding;
        private TorchSharp.Modules.Linear _fc;

        public TextClassificationModel(long vocabSize, long embedDim, long numClass) : base("TextClassification")
        {
            _embedding = EmbeddingBag(vocabSize, embedDim, sparse: false);
            _fc = Linear(embedDim, numClass);
            InitWeights();

            RegisterComponents();
        }

        private void InitWeights()
        {
            var initrange = 0.5;

            init.uniform_(_embedding.weight, -initrange, initrange);
            init.uniform_(_fc.weight, -initrange, initrange);
            init.zeros_(_fc.bias);
        }

        public override Tensor forward(Tensor input, Tensor offsets)
        {
            var t = _embedding.call(input, offsets);
            return _fc.forward(t);
        }
    }
}
