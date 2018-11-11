using System.Collections.Generic;
using System.Linq;
using Vkm.Api.Identification;
using Vkm.Api.Module;
using Vkm.Api.Transition;
using Vkm.Api.VisualEffect;

namespace Vkm.TestProject.Entities
{
    internal class TestModulesService: IModulesService
    {
        private readonly IModule[] _allModules;

        public TestModulesService()
        {
            _allModules = new IModule[]{new TestDeviceFactory(), new TestService(), new TestVisualTransitionService()};
        }

        public IEnumerable<T> GetModules<T>() where T : IModule
        {
            return _allModules.OfType<T>();
        }
    }

    internal class TestVisualTransitionService : IVisualTransitionFactory
    {
        public Identifier Id { get; }
        public string Name => "Test Visual Transition Factory";
        
        public IVisualTransition CreateVisualTransition(TransitionType transitionType)
        {
            return new InstantTransition();
        }
    }
}