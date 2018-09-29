using System;
using System.IO;
using System.Linq;
using Vkm.Api.Basic;
using Vkm.Api.Configurator;
using Vkm.Api.Device;
using Vkm.Api.Identification;
using Vkm.Api.Module;
using Vkm.Api.Options;
using Vkm.Library.ApplicationChangeTransition;
using Vkm.Library.AudioSelect;
using Vkm.Library.Clock;
using Vkm.Library.Common;
using Vkm.Library.CompositeLayout;
using Vkm.Library.Date;
using Vkm.Library.Heartbeat;
using Vkm.Library.IdleTransition;
using Vkm.Library.Mail;
using Vkm.Library.Media;
using Vkm.Library.Numpad;
using Vkm.Library.Power;
using Vkm.Library.Run;
using Vkm.Library.Service.Player;
using Vkm.Library.Service.Weather;
using Vkm.Library.StartupTransition;
using Vkm.Library.Timer;
using Vkm.Library.Volume;
using Vkm.Library.Weather;

namespace Vkm.Library.Config
{
    class LibraryConfig: IConfigurator
    {
        public Identifier Id => new Identifier("Vkm.Library.Configurator");
        public string Name => "Default Library Configurator";
        public IDevice[] Devices { get; set; }


        public void Configure(IOptionsService optionsService)
        {
            Identifier ClockIdentifier = new Identifier("Vkm.DesktopDefaults.Clock");
            Identifier VolumeIdentifier = new Identifier("Vkm.DesktopDefaults.Volume");
            Identifier WeatherIdentifier = new Identifier("Vkm.DesktopDefaults.Weather");
            Identifier MailIdentifier = new Identifier("Vkm.DesktopDefaults.Mail");
            Identifier CalcIdentifier = new Identifier("Vkm.DesktopDefaults.Calc");
            Identifier HeartbeatIdentifier = new Identifier("Vkm.DesktopDefaults.Heartbeat");
            Identifier DateIdentifier = new Identifier("Vkm.DesktopDefaults.Date");
            Identifier MediaIdentifier = new Identifier("Vkm.DesktopDefaults.Media");
            Identifier PowerOffIdentifier = new Identifier("Vkm.DesktopDefaults.PowerOff");
            Identifier AudioSelectIdentifier = new Identifier("Vkm.DesktopDefaults.AudioSelect");
            
            Identifier DefaultCompositeLayout = new Identifier("Vkm.DefaultCompositeLayout.Desktop");
            Identifier DefaultTimerLayout = new Identifier("Vkm.TimerLayout.Default");
            Identifier DefaultApplicationChangeTransitionCalc = new Identifier("Vkm.DefaultApplicationChangeTransition.Calc");
            Identifier DefaultApplicationChangeTransitionExcel = new Identifier("Vkm.DefaultApplicationChangeTransition.Excel");
            Identifier DefaultApplicationChangeTransitionTotalCmd = new Identifier("Vkm.DefaultApplicationChangeTransition.TotalCmd");
            Identifier DefaultIdleTransition = new Identifier("Vkm.DefaultIdleTransition");
            Identifier DefaultStartupTransition = new Identifier("Vkm.DefaultStartupTransition");
            Identifier DefaultNumpadLayout = new Identifier("Vkm.DefaultNumpad.Layout");
            Identifier DefaultScreenSaverLayout = new Identifier("Vkm.DefaultScreensaver.Layout");

            var globalOptions = new GlobalOptions();

            globalOptions.LayoutLoadOptions.InitializationInfos.Add(new ModuleInitializationInfo(NumpadLayoutFactory.Identifier, DefaultNumpadLayout));
            globalOptions.LayoutLoadOptions.InitializationInfos.Add(new ModuleInitializationInfo(ClockLayoutFactory.Identifier, DefaultScreenSaverLayout));
            globalOptions.LayoutLoadOptions.InitializationInfos.Add(new ModuleInitializationInfo(CompositeLayoutFactory.Identifier, DefaultCompositeLayout));
            globalOptions.LayoutLoadOptions.InitializationInfos.Add(new ModuleInitializationInfo(TimerLayoutFactory.Identifier, DefaultTimerLayout));

            globalOptions.TransitionLoadOptions.InitializationInfos.Add(new ModuleInitializationInfo(SimpleStartupTransitionFactory.Identifier, DefaultStartupTransition));
            globalOptions.TransitionLoadOptions.InitializationInfos.Add(new ModuleInitializationInfo(SimpleIdleTransitionFactory.Identifier, DefaultIdleTransition));

            globalOptions.TransitionLoadOptions.InitializationInfos.Add(new ModuleInitializationInfo(SimpleApplicationChangeTransitionFactory.Identifier, DefaultApplicationChangeTransitionCalc));
            globalOptions.TransitionLoadOptions.InitializationInfos.Add(new ModuleInitializationInfo(SimpleApplicationChangeTransitionFactory.Identifier, DefaultApplicationChangeTransitionExcel));
            globalOptions.TransitionLoadOptions.InitializationInfos.Add(new ModuleInitializationInfo(SimpleApplicationChangeTransitionFactory.Identifier, DefaultApplicationChangeTransitionTotalCmd));

            optionsService.SetDefaultOptions(GlobalOptions.Identifier, globalOptions);



            var desktopOptions = new CompositeLayoutOptions();

            desktopOptions.CompositeLayoutElementInfos.Add(new CompositeLayoutElementInfo()
            {
                Location = new Location(0, 0),
                ModuleInfo = new ModuleInitializationInfo(ClockElementFactory.Identifier, ClockIdentifier)
            });

            desktopOptions.CompositeLayoutElementInfos.Add(new CompositeLayoutElementInfo()
            {
                Location = new Location(4, 0),
                ModuleInfo = new ModuleInitializationInfo(VolumeElementFactory.Identifier, VolumeIdentifier)
            });

            desktopOptions.CompositeLayoutElementInfos.Add(new CompositeLayoutElementInfo()
            {
                Location = new Location(0, 1),
                ModuleInfo = new ModuleInitializationInfo(RunElementFactory.Identifier, CalcIdentifier)
            });

            desktopOptions.CompositeLayoutElementInfos.Add(new CompositeLayoutElementInfo()
            {
                Location = new Location(1, 1),
                ModuleInfo = new ModuleInitializationInfo(MediaElementFactory.Identifier, MediaIdentifier)
            });

            desktopOptions.CompositeLayoutElementInfos.Add(new CompositeLayoutElementInfo()
            {
                Location = new Location(2, 1),
                ModuleInfo = new ModuleInitializationInfo(MailElementFactory.Identifier, MailIdentifier)
            });

            desktopOptions.CompositeLayoutElementInfos.Add(new CompositeLayoutElementInfo()
            {
                Location = new Location(2, 2),
                ModuleInfo = new ModuleInitializationInfo(WeatherElementFactory.Identifier, WeatherIdentifier)
            });


            desktopOptions.CompositeLayoutElementInfos.Add(new CompositeLayoutElementInfo()
            {
                Location = new Location(3, 2),
                ModuleInfo = new ModuleInitializationInfo(HeartbeatFactory.Identifier, HeartbeatIdentifier)
            });

            desktopOptions.CompositeLayoutElementInfos.Add(new CompositeLayoutElementInfo()
            {
                Location = new Location(3, 0),
                ModuleInfo = new ModuleInitializationInfo(DateElementFactory.Identifier, DateIdentifier)
            });


            desktopOptions.CompositeLayoutElementInfos.Add(new CompositeLayoutElementInfo()
            {
                Location = new Location(0, 2),
                ModuleInfo = new ModuleInitializationInfo(PowerElementFactory.Identifier, PowerOffIdentifier)
            });

            desktopOptions.CompositeLayoutElementInfos.Add(new CompositeLayoutElementInfo()
            {
                Location = new Location(4, 2),
                ModuleInfo = new ModuleInitializationInfo(AudioSelectElementFactory.Identifier, AudioSelectIdentifier)
            });
            
            
            optionsService.SetDefaultOptions(DefaultCompositeLayout, desktopOptions);



            var runOptions = new RunOptions();

            runOptions.Executable = "calc.exe";
            runOptions.Symbol = FontAwesomeRes.fa_calculator;

            optionsService.SetDefaultOptions(CalcIdentifier, runOptions);

            var weatherServiceOptions = new OpenWeatherOptions { OpenWeatherApiKey = "3e1cbac94caf82e428a662bc15b2fe9e" };
            optionsService.SetDefaultOptions(OpenWeatherService.Identifier, weatherServiceOptions);

            var weatherOptions = new WeatherOptions() {Place = "Dachau"};
            optionsService.SetDefaultOptions(WeatherIdentifier, weatherOptions);
            
            var applicationTransitionOptions = new ApplicationChangeTransitionOptions(Devices.FirstOrDefault()?.Id ?? new Identifier());
            applicationTransitionOptions.Process = "Calculator";
            applicationTransitionOptions.LayoutId = DefaultNumpadLayout;
            optionsService.SetDefaultOptions(DefaultApplicationChangeTransitionCalc, applicationTransitionOptions);

            applicationTransitionOptions = new ApplicationChangeTransitionOptions(Devices.FirstOrDefault()?.Id ?? new Identifier());
            applicationTransitionOptions.Process = "EXCEL";
            applicationTransitionOptions.LayoutId = DefaultNumpadLayout;
            optionsService.SetDefaultOptions(DefaultApplicationChangeTransitionExcel, applicationTransitionOptions);

            applicationTransitionOptions = new ApplicationChangeTransitionOptions(Devices.FirstOrDefault()?.Id ?? new Identifier());
            applicationTransitionOptions.Process = "TOTALCMD64";
            applicationTransitionOptions.LayoutId = DefaultNumpadLayout;
            optionsService.SetDefaultOptions(DefaultApplicationChangeTransitionTotalCmd, applicationTransitionOptions);


            var idleOptions = new IdleTransitionOptions(Devices.FirstOrDefault()?.Id ?? new Identifier()) {LayoutId = DefaultScreenSaverLayout};
            optionsService.SetDefaultOptions(DefaultIdleTransition, idleOptions);


            var startupTransitionOptions = new StartupTransitionOptions(Devices.FirstOrDefault()?.Id ?? new Identifier()) {LayoutId = DefaultCompositeLayout};
            optionsService.SetDefaultOptions(DefaultStartupTransition, startupTransitionOptions);

            var clockOptions = new ClockElementOptions() {TimerLayoutIdentifier = DefaultTimerLayout};
            optionsService.SetDefaultOptions(ClockIdentifier, clockOptions);

            var powerOffOptions = new PowerOptions() {PressToActionTimeout = TimeSpan.FromSeconds(1), Action = PowerAction.PowerOff};
            optionsService.SetDefaultOptions(PowerOffIdentifier, powerOffOptions);

            var audioSelectOptions = new AudioSelectOptions();
            audioSelectOptions.Names.Add("{0.0.0.00000000}.{4eff022d-8d54-413f-9020-581e6654434b}", "Наушники");
            audioSelectOptions.Names.Add("{0.0.0.00000000}.{5325d718-fd5e-479b-907c-a4873af76102}", "Камера");
            audioSelectOptions.Names.Add("{0.0.0.00000000}.{ba74e64f-2267-4701-ab2e-2cc8685c124d}", "Колонки");
            optionsService.SetDefaultOptions(AudioSelectIdentifier, audioSelectOptions);
            
            var lastFmOptions = new LastFmOptions() {Domain = "http://ws.audioscrobbler.com", ApiKey = "d1a52a26a6f62158fbd86090441f81fb"};
            optionsService.SetDefaultOptions(LastFmAlbumCoverService.Identifier, lastFmOptions);

            var amipOptions = new AmipOptions() {Separator = new[] {" ||| "}, Filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "amip.txt")};
            optionsService.SetDefaultOptions(AmipPlayerService.Identifier, amipOptions);
        }

    }
}
