using System;
using System.Collections.Generic;
using System.Linq;
using Mvvm;
using Orimath.Core;
using Orimath.FoldingInstruction;
using Orimath.Plugins;
using ApplicativeProperty;

namespace Orimath.Basics.View.ViewModels
{
    public class NetViewModel : NotifyPropertyChanged
    {
        private readonly IPaperModel _paper;
        private readonly IViewPointConverter _pointConverter;
        private readonly IDispatcher _dispatcher;
        public ResettableObservableCollection<NetLineViewModel> Lines { get; } = new();

        public NetViewModel(IPaperModel paper, IDispatcher dispatcher)
        {
            _paper = paper;
            _pointConverter = new ViewPointConverter(128.0, -128.0, -0.5, 127.5);
            _dispatcher = dispatcher;

            _paper.Layers.Subscribe(_ => _dispatcher.OnUIAsync(() =>
            {
                IEnumerable<NetLineViewModel> GetViewModels()
                {
                    var layers = _paper.Layers.ToArray();
                    for (var i = 0; i < layers.Length; i++)
                    {
                        foreach (var edge in layers[i].OriginalEdges)
                        {
                            if (edge.Inner)
                            {
                                var ok = layers
                                    .Take(i)
                                    .SelectMany(x => x.OriginalEdges)
                                    .All(e => !e.Inner || !NearlyEquatable.NearlyEquals(edge.Line, e.Line));
                                if (ok)
                                {
                                    yield return new NetLineViewModel(edge.Line, _pointConverter,
                                        layers[i].LayerType == LayerType.BackSide ? InstructionColor.Blue : InstructionColor.Red);
                                }
                            }
                            else
                            {
                                yield return new NetLineViewModel(edge.Line, _pointConverter, InstructionColor.Black);
                            }
                        }
                    }
                    yield break;
                }

                Lines.Reset(GetViewModels());
            }));
        }
    }
}
