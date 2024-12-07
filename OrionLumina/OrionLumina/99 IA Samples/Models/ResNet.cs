// Copyright (c) .NET Foundation and Contributors.  All Rights Reserved.  See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using TorchSharp;
using static TorchSharp.torch;
using static TorchSharp.torch.nn;

namespace Models
{
    /// <summary>
    /// Modified version of ResNet to classify CIFAR10 32x32 images.
    /// </summary>
    public class ResNet : Module<Tensor, Tensor>
    {
        // The code here is is loosely based on https://github.com/kuangliu/pytorch-cifar/blob/master/models/resnet.py
        // Licence and copypright notice at: https://github.com/kuangliu/pytorch-cifar/blob/master/LICENSE

        private readonly long[] _planes = [64, 128, 128, 256, 256, 512, 512, 512, 512, 512, 512, 1024, 1024];
        private readonly long[] _strides = [1, 2, 1, 2, 1, 2, 1, 1, 1, 1, 1, 2, 1];

        private readonly Module<Tensor, Tensor> _layers;
        private int _inPlanes = 64;

        public static ResNet ResNet18(int numClasses, Device device = null)
        {
            return new ResNet(
                "ResNet18",
                (name, inPlanes, planes, stride) => new BasicBlock(name, inPlanes, planes, stride),
                BasicBlock.Expansion, [2, 2, 2, 2],
                numClasses,
                device); 
        }

        public static ResNet ResNet34(int numClasses, Device device = null)
        {
            return new ResNet(
                "ResNet34",
                (name, inPlanes, planes, stride) => new BasicBlock(name, inPlanes, planes, stride),
                BasicBlock.Expansion, [3, 4, 6, 3],
                numClasses,
                device);
        }

        public static ResNet ResNet50(int numClasses, Device device = null)
        {
            return new ResNet(
                "ResNet50",
                (name, inPlanes, planes, stride) => new Bottleneck(name, inPlanes, planes, stride),
                Bottleneck.Expansion, [3, 4, 6, 3],
                numClasses,
                device);
        }

        public static ResNet ResNet101(int numClasses, Device device = null)
        {
            return new ResNet(
                "ResNet101",
                (name, inPlanes, planes, stride) => new Bottleneck(name, inPlanes, planes, stride),
                Bottleneck.Expansion, [3, 4, 23, 3],
                numClasses,
                device);
        }

        public static ResNet ResNet152(int numClasses, Device device = null)
        {
            return new ResNet(
                "ResNet101",
                (name, inPlanes, planes, stride) => new Bottleneck(name, inPlanes, planes, stride),
                Bottleneck.Expansion, [3, 4, 36, 3],
                numClasses,
                device);
        }

        public ResNet(string name, Func<string, int,int,int, Module<Tensor, Tensor>> block, int expansion, IList<int> numBlocks, int numClasses, Device device = null) : base(name)
        {
            if (_planes.Length != _strides.Length) throw new ArgumentException("'planes' and 'strides' must have the same length.");

            var modules = new List<(string, Module<Tensor, Tensor>)>();

            modules.Add(($"conv2d-first", Conv2d(3, 64, kernelSize: 3, stride: 1, padding: 1, bias: false)));
            modules.Add(($"bnrm2d-first", BatchNorm2d(64)));
            modules.Add(($"relu-first", ReLU(inplace:true)));
            MakeLayer(modules, block, expansion, 64, numBlocks[0], 1);
            MakeLayer(modules, block, expansion, 128, numBlocks[1], 2);
            MakeLayer(modules, block, expansion, 256, numBlocks[2], 2);
            MakeLayer(modules, block, expansion, 512, numBlocks[3], 2);
            modules.Add(("avgpool", AvgPool2d([4, 4])));
            modules.Add(("flatten", Flatten()));
            modules.Add(($"linear", Linear(512 * expansion, numClasses)));

            _layers = Sequential(modules);

            RegisterComponents();

            if (device != null && device.type != DeviceType.CPU)
                this.to(device);
        }

        private void MakeLayer(List<(string, Module<Tensor, Tensor>)> modules, Func<string, int, int, int, Module<Tensor, Tensor>> block, int expansion, int planes, int numBlocks, int stride)
        {
            var strides = new List<int>();
            strides.Add(stride);
            for (var i = 0; i < numBlocks-1; i++) { strides.Add(1); }

            for (var i = 0; i < strides.Count; i++) {
                var s = strides[i];
                modules.Add(($"blck-{planes}-{i}", block($"blck-{planes}-{i}", _inPlanes, planes, s)));
                _inPlanes = planes * expansion;
            }
        }

        public override Tensor forward(Tensor input)
        {
            return _layers.forward(input);
        }

        class BasicBlock : Module<Tensor, Tensor>
        {
            public BasicBlock (string name, int inPlanes, int planes, int stride) : base(name)
            {
                var modules = new List<(string, Module<Tensor, Tensor>)>();

                modules.Add(($"{name}-conv2d-1", Conv2d(inPlanes, planes, kernelSize: 3, stride: stride, padding: 1, bias: false)));
                modules.Add(($"{name}-bnrm2d-1", BatchNorm2d(planes)));
                modules.Add(($"{name}-relu-1", ReLU(inplace: true)));
                modules.Add(($"{name}-conv2d-2", Conv2d(planes, planes, kernelSize: 3, stride: 1, padding: 1, bias: false)));
                modules.Add(($"{name}-bnrm2d-2", BatchNorm2d(planes)));

                _layers = Sequential(modules);

                if (stride != 1 || inPlanes != Expansion*planes) {
                    _shortcut = Sequential(
                        ($"{name}-conv2d-3", Conv2d(inPlanes, Expansion * planes, kernelSize: 1, stride: stride, bias: false)),
                        ($"{name}-bnrm2d-3", BatchNorm2d(Expansion * planes)));
                }
                else {
                    _shortcut = Sequential();
                }

                modules.Add(($"{name}-relu-2", ReLU(inplace: true)));

                RegisterComponents();
            }

            public override Tensor forward(Tensor t)
            {
                var x = _layers.forward(t);
                var y = _shortcut.forward(t);
                return x.add_(y).relu_();
            }

            public static int Expansion = 1;

            private readonly Module<Tensor, Tensor> _layers;
            private readonly Module<Tensor, Tensor> _shortcut;
        }

        class Bottleneck : Module<Tensor, Tensor>
        {
            public Bottleneck(string name, int inPlanes, int planes, int stride) : base(name)
            {
                var modules = new List<(string, Module<Tensor, Tensor>)>();

                modules.Add(($"{name}-conv2d-1", Conv2d(inPlanes, planes, kernelSize: 1, bias: false)));
                modules.Add(($"{name}-bnrm2d-1", BatchNorm2d(planes)));
                modules.Add(($"{name}relu-1", ReLU(inplace:true)));
                modules.Add(($"{name}-conv2d-2", Conv2d(planes, planes, kernelSize: 3, stride: stride, padding: 1, bias: false)));
                modules.Add(($"{name}-bnrm2d-2", BatchNorm2d(planes)));
                modules.Add(($"{name}relu-2", ReLU(inplace: true)));
                modules.Add(($"{name}-conv2d-3", Conv2d(planes, Expansion * planes, kernelSize: 1, bias: false)));
                modules.Add(($"{name}-bnrm2d-3", BatchNorm2d(Expansion * planes)));

                _layers = Sequential(modules);

                if (stride != 1 || inPlanes != Expansion * planes) {
                    _shortcut = Sequential(
                        ($"{name}-conv2d-4", Conv2d(inPlanes, Expansion * planes, kernelSize: 1, stride: stride, bias: false)),
                        ($"{name}-bnrm2d-4", BatchNorm2d(Expansion * planes)));
                } else {
                    _shortcut = Sequential();
                }

                RegisterComponents();
            }

            public override Tensor forward(Tensor t)
            {
                var x = _layers.forward(t);
                var y = _shortcut.forward(t);
                return x.add_(y).relu_();
            }

            public static int Expansion = 4;

            private readonly Module<Tensor, Tensor> _layers;
            private readonly Module<Tensor, Tensor> _shortcut;
        }
    }
}
