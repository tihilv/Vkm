using System.Collections.Generic;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Data;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Library.Common;
using Vkm.Library.Interfaces.Service;

namespace Vkm.Library.Run
{
    internal class TaskbarLayout: LayoutBase
    {
        private List<RunElement> _elements;

        private IProcessService _processService;

        public TaskbarLayout(Identifier identifier) : base(identifier)
        {
            _elements = new List<RunElement>();
        }

        public override void Init()
        {
            base.Init();

            _processService = GlobalContext.GetServices<IProcessService>().First();

            AddElement(new Location(4, 2), GlobalContext.InitializeEntity(new BackElement()));

        }

        public override void EnterLayout(LayoutContext layoutContext, ILayout previousLayout)
        {
            base.EnterLayout(layoutContext, previousLayout);

            FillElements();
        }

        void FillElements()
        {
            using (LayoutContext.PauseDrawing())
            {
                foreach (var element in _elements)
                    RemoveElement(element);

                _elements.Clear();

                var processes = _processService.GetProcessesWithWindows();

                foreach (var processInfo in processes)
                {
                    var element = GlobalContext.InitializeEntity(new RunElement(new Identifier($"Vkm.RunProcess.N{processInfo.Handle}")));
                    element.InitOptions(new RunOptions() {Executable = processInfo.ExecutableFileName});
                    element.SetRunning(processInfo.MainWindowHandle);
                    _elements.Add(element);
                }

                AddElementsInRectangle(_elements, 0, 0, (byte) (LayoutContext.IconSize.Width - 1), (byte) (LayoutContext.IconSize.Height - 1));
            }
        }
    }
}
