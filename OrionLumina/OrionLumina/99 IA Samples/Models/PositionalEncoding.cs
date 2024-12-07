using System;
using TorchSharp;

namespace Models;

class PositionalEncoding : torch.nn.Module<torch.Tensor, torch.Tensor>
{
    private torch.nn.Module<torch.Tensor, torch.Tensor> _dropout;
    private torch.Tensor _pe;

    public PositionalEncoding(long dmodel, double dropout, int maxLen = 5000) : base("PositionalEncoding")
    {
        this._dropout = torch.nn.Dropout(dropout);
        var pe = torch.zeros(new long[] { maxLen, dmodel });
        var position = torch.arange(0, maxLen, 1).unsqueeze(1);
        var divTerm = (torch.arange(0, dmodel, 2) * (-Math.Log(10000.0) / dmodel)).exp();
        pe[torch.TensorIndex.Ellipsis, torch.TensorIndex.Slice(0, null, 2)] = (position * divTerm).sin();
        pe[torch.TensorIndex.Ellipsis, torch.TensorIndex.Slice(1, null, 2)] = (position * divTerm).cos();
        this._pe = pe.unsqueeze(0).transpose(0, 1);

        RegisterComponents();
    }

    public override torch.Tensor forward(torch.Tensor t)
    {
        var x = t + _pe[torch.TensorIndex.Slice(null, t.shape[0]), torch.TensorIndex.Slice()];
        return _dropout.forward(x);
    }
}