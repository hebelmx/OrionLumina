// Copyright (c) .NET Foundation and Contributors.  All Rights Reserved.  See LICENSE in the project root for license information.

using System;
using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;

namespace Models
{
    /// <summary>
    /// This example is based on the PyTorch tutorial at:
    /// 
    /// https://pytorch.org/tutorials/beginner/transformer_tutorial.html
    ///
    /// </summary>

    public class TransformerModel : Module<Tensor, Tensor, Tensor>
    {
        private TorchSharp.Modules.TransformerEncoder _transformerEncoder;
        private PositionalEncoding _posEncoder;
        private TorchSharp.Modules.Embedding _encoder;
        private TorchSharp.Modules.Linear _decoder;

        private long _ninputs;
        private Device _device;

        public TransformerModel(long ntokens, long ninputs, long nheads, long nhidden, long nlayers, double dropout = 0.5) : base("Transformer")
        {
            this._ninputs = ninputs;

            _posEncoder = new PositionalEncoding(ninputs, dropout);
            var encoderLayers = TransformerEncoderLayer(ninputs, nheads, nhidden, dropout);
            _transformerEncoder = TransformerEncoder(encoderLayers, nlayers);
            _encoder = Embedding(ntokens, ninputs);
            _decoder = Linear(ninputs, ntokens);
            InitWeights();

            RegisterComponents();
        }

        public Tensor GenerateSquareSubsequentMask(long size)
        {
            var mask = (torch.ones(new long[] { size, size }) == 1).triu().transpose(0, 1);
            return mask.to_type(ScalarType.Float32)
                .masked_fill(mask == 0, float.NegativeInfinity)
                .masked_fill(mask == 1, 0.0f).to(_device);
        }

        private void InitWeights()
        {
            var initrange = 0.1;

            init.uniform_(_encoder.weight, -initrange, initrange);
            init.zeros_(_decoder.bias);
            init.uniform_(_decoder.weight, -initrange, initrange);
        }

        public override Tensor forward(Tensor t, Tensor mask)
        {
            using var src = _posEncoder.forward(_encoder.forward(t) * MathF.Sqrt(_ninputs));
            using var enc = _transformerEncoder.call(src, mask);
            return _decoder.forward(enc);
        }

        public TransformerModel To(Device device)
        {
            this.to<TransformerModel>(device);
            this._device = device;
            return this;
        }
    }
}
