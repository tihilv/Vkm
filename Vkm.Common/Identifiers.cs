using Vkm.Api.Identification;

namespace Vkm.Common
{
    public static class Identifiers
    {
        public static readonly Identifier DefaultScreenSaverFactory = new Identifier("Vkm.DefaultScreensaver.Factory");
        public static readonly Identifier DefaultScreenSaverLayout = new Identifier("Vkm.DefaultScreensaver.Layout");

        public static readonly Identifier DefaultNumpadFactory = new Identifier("Vkm.DefaultNumpad.Factory");
        public static readonly Identifier DefaultNumpadLayout = new Identifier("Vkm.DefaultNumpad.Layout");

        public static readonly Identifier DefaultStartupTransitionFactory = new Identifier("Vkm.DefaultStartupTransition.Factory");
        public static readonly Identifier DefaultStartupTransition = new Identifier("Vkm.DefaultStartupTransition");

        public static readonly Identifier DefaultIdleTransitionFactory = new Identifier("Vkm.DefaultIdleTransition.Factory");
        public static readonly Identifier DefaultIdleTransition = new Identifier("Vkm.DefaultIdleTransition");

        public static readonly Identifier DefaultApplicationChangeTransitionFactory = new Identifier("Vkm.DefaultApplicationChangeTransition.Factory");
        public static readonly Identifier DefaultApplicationChangeTransitionCalc = new Identifier("Vkm.DefaultApplicationChangeTransition.Calc");
        public static readonly Identifier DefaultApplicationChangeTransitionExcel = new Identifier("Vkm.DefaultApplicationChangeTransition.Excel");
        public static readonly Identifier DefaultApplicationChangeTransitionTotalCmd = new Identifier("Vkm.DefaultApplicationChangeTransition.TotalCmd");

        public static readonly Identifier DefaultCompositeLayoutFactory = new Identifier("Vkm.DefaultCompositeLayout.Factory");
        public static readonly Identifier DefaultCompositeLayout = new Identifier("Vkm.DefaultCompositeLayout.Desktop");
    }
}
