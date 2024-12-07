// Copyright (c) .NET Foundation and Contributors.  All Rights Reserved.  See LICENSE in the project root for license information.

using TorchSharp;
using static TorchSharp.torch;

using static TorchSharp.torch.nn;

namespace Models
{
    public class Model : Module<Tensor,Tensor>
    {
        private Module<Tensor, Tensor> _conv1 = Conv2d(1, 32, 3);
        private Module<Tensor, Tensor> _conv2 = Conv2d(32, 64, 3);
        private Module<Tensor, Tensor> _fc1 = Linear(9216, 128);
        private Module<Tensor, Tensor> _fc2 = Linear(128, 10);

        // These don't have any parameters, so the only reason to instantiate
        // them is performance, since they will be used over and over.
        private Module<Tensor, Tensor> _pool1 = MaxPool2d(kernelSize: [2, 2]);

        private Module<Tensor, Tensor> _relu1 = ReLU();
        private Module<Tensor, Tensor> _relu2 = ReLU();
        private Module<Tensor, Tensor> _relu3 = ReLU();

        private Module<Tensor, Tensor> _dropout1 = Dropout(0.25);
        private Module<Tensor, Tensor> _dropout2 = Dropout(0.5);

        private Module<Tensor, Tensor> _flatten = Flatten();
        private Module<Tensor, Tensor> _logsm = LogSoftmax(1);

        public Model(string name, torch.Device device = null) : base(name)
        {
            RegisterComponents();

            if (device != null && device.type != DeviceType.CPU)
                this.to(device);
        }

        public override Tensor forward(Tensor input)
        {
            var l11 = _conv1.forward(input);
            var l12 = _relu1.forward(l11);

            var l21 = _conv2.forward(l12);
            var l22 = _relu2.forward(l21);
            var l23 = _pool1.forward(l22);
            var l24 = _dropout1.forward(l23);

            var x = _flatten.forward(l24);

            var l31 = _fc1.forward(x);
            var l32 = _relu3.forward(l31);
            var l33 = _dropout2.forward(l32);

            var l41 = _fc2.forward(l33);

            return _logsm.forward(l41);
        }
    }
}
