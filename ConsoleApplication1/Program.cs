﻿using System;
using System.ComponentModel;
using System.Net;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Common.Logging;
using Jal.Finder.Atrribute;
using Jal.Finder.Impl;
using Jal.Locator.CastleWindsor.Installer;
using Jal.Router.AzureServiceBus.Installer;
using Jal.Router.AzureStorage.Installer;
using Jal.Router.Impl;
using Jal.Router.Installer;
using Jal.Router.Interface;
using Jal.Router.Logger.Installer;
using Jal.Router.Model;
using Jal.Router.Tests.Impl;
using Jal.Router.Tests.Model;
using Jal.Settings.Installer;
using Microsoft.ServiceBus.Messaging;
using Component = Castle.MicroKernel.Registration.Component;


namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            AssemblyFinder.Current = AssemblyFinder.Builder.UsePath(AppDomain.CurrentDomain.BaseDirectory).Create;
            IWindsorContainer container = new WindsorContainer();
            var assemblies = AssemblyFinder.Current.GetAssembliesTagged<AssemblyTagAttribute>();
            container.Kernel.Resolver.AddSubResolver(new ArrayResolver(container.Kernel));
            container.Install(new RouterInstaller(assemblies));
            container.Install(new ServiceLocatorInstaller());
            container.Install(new SettingsInstaller());
            container.Register(Component.For(typeof(IMessageHandler<Message>)).ImplementedBy(typeof(MessageHandler)).Named(typeof(MessageHandler).FullName).LifestyleSingleton());
            container.Register(Component.For(typeof(IMessageHandler<Message1>)).ImplementedBy(typeof(Message1Handler)).Named(typeof(Message1Handler).FullName).LifestyleSingleton());
            container.Register(Component.For(typeof(IMessageHandler<Message>)).ImplementedBy(typeof(OtherMessageHandler)).Named(typeof(OtherMessageHandler).FullName).LifestyleSingleton());
            container.Install(new AzureServiceBusRouterInstaller());
            container.Install(new AzureStorageInstaller("DefaultEndpointsProtocol=https;AccountName=narwhalappssaeus001;AccountKey=xn2flH2joqs8LM0JKQXrOAWEEXc/I4e9AF873p1W/2grHSht8WEIkBbbl3PssTatuRCLlqMxbkvhKN9VmcPsFA=="));
            container.Install(new RouterLoggerInstaller());
            container.Register(Component.For(typeof(ILog)).Instance(LogManager.GetLogger("Cignium.Enigma.App")));
            var sagabrokered = container.Resolve<ISagaRouter<BrokeredMessage>>();

            //var original = ServicePointManager.ServerCertificateValidationCallback;

            //ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) =>
            //{
            //    var request = o as HttpWebRequest;



            //    return true;
            //};


            var bm = new BrokeredMessage(@"{""Name"":""Raul""}");
            bm.Properties.Add("origin", "AB");


            sagabrokered.Route<Message>(bm, "saga");

            var bm1 = new BrokeredMessage(@"{""Name1"":""Raul Naupari""}");
            bm1.Properties.Add("origin", "ABC");
            //{"partitionkey":"20170822_saga_f0dd186a-3043-4f32-b437-3d5d283b8f88","rowkey":"e5a5c8be-ce35-45e4-9a8c-3d8b539513dc"}
            //{ "partitionkey":"20170821_944e53c5-b3eb-43f1-bc87-e5bd4260c21a","rowkey":"f274f66c-095e-48ce-8f3c-6fc7a2b0ef39"}
            //{"partitionkey":"20170821_737d3a2e-b22c-4ec0-92d9-82474df6757f","rowkey":"bd09b4ed-6485-4293-b568-8e021ec8179c"}
            bm1.Properties.Add("partitionkey", "20170822_saga_f0dd186a-3043-4f32-b437-3d5d283b8f88");
            bm1.Properties.Add("rowkey", "e5a5c8be-ce35-45e4-9a8c-3d8b539513dc");
            sagabrokered.Route<Message1>(bm1, "saga");
        }
    }
}