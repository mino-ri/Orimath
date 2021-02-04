﻿using System;
using Orimath.Controls;
using Orimath.Plugins;
using ApplicativeProperty;

namespace Orimath.Basics.View.ViewModels
{
    public class MeasureViewModel : NotifyPropertyChanged
    {
        private readonly IPaperModel _paper;
        private readonly IViewPointConverter _pointConverter;
        private readonly IDispatcher _dispatcher;

        public MeasureViewModel(IPaperModel paper, IViewPointConverter pointConverter, IDispatcher dispatcher)
        {
            _paper = paper;
            _pointConverter = pointConverter;
            _dispatcher = dispatcher;

            _paper.SelectedEdges.Subscribe(values => _dispatcher.OnUIAsync(() =>
            {
                SelectedEdges = Array.ConvertAll(values, e => new EdgeViewModel(null, e, _pointConverter));
                OnPropertyChanged(nameof(SelectedEdges));
            }));
            _paper.SelectedLines.Subscribe(values => _dispatcher.OnUIAsync(() =>
            {
                SelectedLines = Array.ConvertAll(values, l => new LineViewModel(null, l, _pointConverter));
                OnPropertyChanged(nameof(SelectedLines));
            }));
            _paper.SelectedPoints.Subscribe(values => _dispatcher.OnUIAsync(() =>
            {
                SelectedPoints = Array.ConvertAll(values, p => new PointViewModel(null, p, _pointConverter));
                OnPropertyChanged(nameof(SelectedPoints));
            }));
        }

        public EdgeViewModel[] SelectedEdges { get; private set; } = Array.Empty<EdgeViewModel>();
        public LineViewModel[] SelectedLines { get; private set; } = Array.Empty<LineViewModel>();
        public PointViewModel[] SelectedPoints { get; private set; } = Array.Empty<PointViewModel>();
    }
}
