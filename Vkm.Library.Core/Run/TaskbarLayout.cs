using System.Collections.Generic;
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

        public TaskbarLayout(Identifier identifier) : base(identifier)
        {
            _elements = new List<RunElement>();
        }

        public override void Init()
        {
            base.Init();

            _processService = GlobalContext.GetServices<IProcessService>().First();
            _currentProcessService = GlobalContext.GetServices<ICurrentProcessService>().First();

            AddElement(new Location(4, 2), GlobalContext.InitializeEntity(new BackElement()));

        }

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);
            _currentProcessService.ProcessEnter += OnProcessEnter;

            FillElements();
        }

        public override void LeaveLayout()
        {
            _currentProcessService.ProcessEnter -= OnProcessEnter;
            base.LeaveLayout();
        }

        private void OnProcessEnter(object sender, ProcessEventArgs e)
        {
            _currentProcessId = e.ProcessId;
            FillElements();
        }

        private object _fillLock = new object();
        void FillElements()
        {
            lock (_fillLock)
            {
                using (LayoutContext.PauseDrawing())
                {
                    foreach (var element in _elements)
                        RemoveElement(element);

                    var processes = _processService.GetProcessesWithWindows().OrderBy(p => p.MainWindowText).Take(LayoutContext.ButtonCount.Width * LayoutContext.ButtonCount.Height - 1);

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
