﻿using System.Collections.Generic;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Library.Common;
using Vkm.Library.Interfaces.Service;
using Vkm.Library.Interfaces.Services;

namespace Vkm.Library.Run
{
    internal class TaskbarLayout: LayoutBase
    {
        private readonly List<RunElement> _elements;

        private int _currentProcessId;

        private IProcessService _processService;
        private ICurrentProcessService _currentProcessService;

        private BackElement _backElement;

        public TaskbarLayout(Identifier identifier) : base(identifier)
        {
            _elements = new List<RunElement>();
        }

        public override void Init()
        {
            base.Init();

            _processService = GlobalContext.GetServices<IProcessService>().First();
            _currentProcessService = GlobalContext.GetServices<ICurrentProcessService>().First();

            _backElement = GlobalContext.InitializeEntity(new BackElement());

        }

        protected override void OnEnteredLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            _currentProcessService.ProcessEnter += OnProcessEnter;

            AddElement(new Location(layoutContext.ButtonCount.Width-1, layoutContext.ButtonCount.Height-1), _backElement);
            FillElements(layoutContext);
        }

        protected override void OnLeavingLayout()
        {
            RemoveElement(_backElement);
            _currentProcessService.ProcessEnter -= OnProcessEnter;
        }

        private void OnProcessEnter(object sender, ProcessEventArgs e)
        {
            _currentProcessId = e.ProcessId;

            WithLayout(FillElements);
        }

        private object _fillLock = new object();
        void FillElements(LayoutContext layoutContext)
        {
            lock (_fillLock)
            {
                using (layoutContext.PauseDrawing())
                {
                    foreach (var element in _elements)
                        RemoveElement(element);

                    var processes = _processService.GetProcessesWithWindows().OrderBy(p => p.MainWindowText).Take(layoutContext.ButtonCount.Width * layoutContext.ButtonCount.Height - 1);

                    _elements.Clear();

                    foreach (var processInfo in processes)
                    {
                        var element = GlobalContext.InitializeEntity(new RunElement(new Identifier($"Vkm.RunProcess.N{processInfo.Handle}")));
                        element.InitOptions(new RunOptions() {Executable = processInfo.ExecutableFileName});
                        element.SetRunning(processInfo.Id, processInfo.Id == _currentProcessId);
                        _elements.Add(element);
                    }

                    AddElementsInRectangle(_elements);
                }
            }
        }
    }
}
