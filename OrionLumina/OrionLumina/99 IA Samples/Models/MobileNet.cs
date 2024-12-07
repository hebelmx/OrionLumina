// Copyright (c) .NET Foundation and Contributors.  All Rights Reserved.  See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;

namespace Models
{
    /// <summary>
    /// Modified version of MobileNet to classify CIFAR10 32x32 images.
    /// </summary>
    /// <remarks>
    /// With an unaugmented CIFAR-10 data set, the author of this saw training converge
    /// at roughly 75% accuracy on the test set, over the course of 1500 epochs.
    /// </remarks>
    public class MobileNet : Module<Tensor, Tensor>
    {
        // The code here is is loosely based on https://github.com/kuangliu/pytorch-cifar/blob/master/models/mobilenet.py
        // Licence and copypright notice at: https://github.com/kuangliu/pytorch-cifar/blob/master/LICENSE

        private readonly long[] _planes = [64, 128, 128, 256, 256, 512, 512, 512, 512, 512, 512, 1024, 1024];
        private readonly long[] _strides = [1, 2, 1, 2, 1, 2, 1, 1, 1, 1, 1, 2, 1];

        private readonly Module<Tensor, Tensor> _layers;

        public MobileNet(string name, int numClasses, Device device = null) : base(name)
        {
            if (_planes.Length != _strides.Length) throw new ArgumentException("'planes' and 'strides' must have the same length.");

            var modules = new List<(string, Module<Tensor,Tensor>)>();

            modules.Add(($"conv2d-first", Conv2d(3, 32, kernelSize: 3, stride: 1, padding: 1, bias: false)));
            modules.Add(($"bnrm2d-first", BatchNorm2d(32)));
            modules.Add(($"relu-first", ReLU()));
            MakeLayers(modules, 32);
            modules.Add(("avgpool", AvgPool2d([2, 2])));
            modules.Add(("flatten", Flatten()));
            modules.Add(($"linear", Linear(_planes[^1], numClasses)));

            _layers = Sequential(modules);

            RegisterComponents();

            if (device != null && device.type != DeviceType.CPU)
                this.to(device);
        }

        private void MakeLayers(List<(string, Module<Tensor, Tensor>)> modules, long inPlanes)
        {

            for (var i = 0; i < _strides.Length; i++) {
                var outPlanes = _planes[i];
                var stride = _strides[i];

                modules.Add(($"conv2d-{i}a", Conv2d(inPlanes, inPlanes, kernelSize: 3, stride: stride, padding: 1, groups: inPlanes, bias: false)));
                modules.Add(($"bnrm2d-{i}a", BatchNorm2d(inPlanes)));
                modules.Add(($"relu-{i}a", ReLU()));
                modules.Add(($"conv2d-{i}b", Conv2d(inPlanes, outPlanes, kernelSize: 1L, stride: 1L, padding: 0L, bias: false)));
                modules.Add(($"bnrm2d-{i}b", BatchNorm2d(outPlanes)));
                modules.Add(($"relu-{i}b", ReLU()));

                inPlanes = outPlanes;
            }
        }

        public override Tensor forward(Tensor input)
        {
            return _layers.forward(input);
        }
    }
}
