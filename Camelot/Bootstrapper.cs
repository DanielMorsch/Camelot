using System;
using ApplicationDispatcher.Implementations;
using ApplicationDispatcher.Interfaces;
using Camelot.Factories.Implementations;
using Camelot.Factories.Interfaces;
using Camelot.FileSystemWatcherWrapper.Implementations;
using Camelot.FileSystemWatcherWrapper.Interfaces;
using Camelot.Services.Behaviors.Implementations;
using Camelot.Services.Implementations;
using Camelot.Services.Interfaces;
using Camelot.Services.Operations.Implementations;
using Camelot.Services.Operations.Interfaces;
using Camelot.TaskPool.Interfaces;
using Camelot.ViewModels;
using Camelot.ViewModels.MainWindow;
using Splat;

namespace Camelot
{
    public static class Bootstrapper
    {
        public static void Register(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            RegisterServices(services, resolver);
            RegisterViewModels(services, resolver);
        }

        private static void RegisterServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IFileService>(() => new FileService());
            services.RegisterLazySingleton<IFileSystemWatcherWrapperFactory>(() => new FileSystemWatcherWrapperFactory());
            services.RegisterLazySingleton<ITaskPool>(() => new TaskPool.Implementations.TaskPool(Environment.ProcessorCount));
            services.Register<IOperationsFactory>(() => new OperationsFactory(
                resolver.GetService<ITaskPool>()));
            services.RegisterLazySingleton<IFileSystemWatchingService>(() => new FileSystemWatchingService(
                resolver.GetService<IFileSystemWatcherWrapperFactory>()));
            services.RegisterLazySingleton<IFilesSelectionService>(() => new FilesSelectionService());
            services.RegisterLazySingleton<IDirectoryService>(() => new DirectoryService());
            services.RegisterLazySingleton<IFileOpeningService>(() => new FileOpeningService());
            services.RegisterLazySingleton<IFileSystemWatcherWrapperFactory>(() => new FileSystemWatcherWrapperFactory());
            services.Register<IFileSystemWatchingService>(() => new FileSystemWatchingService(
                resolver.GetService<IFileSystemWatcherWrapperFactory>()));
            services.RegisterLazySingleton(() => new FileOpeningBehavior(
                resolver.GetService<IFileOpeningService>()));
            services.RegisterLazySingleton(() => new DirectoryOpeningBehavior(
                resolver.GetService<IDirectoryService>()));
            services.RegisterLazySingleton<IApplicationDispatcher>(() => new AvaloniaDispatcher());
        }

        private static void RegisterViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IFileViewModelFactory>(() => new FileViewModelFactory(
                resolver.GetService<FileOpeningBehavior>(),
                resolver.GetService<DirectoryOpeningBehavior>()));
            services.Register(() => new OperationsViewModel(
                resolver.GetService<IFilesSelectionService>(),
                resolver.GetService<IOperationsFactory>()));
            services.Register(() => new FilesPanelViewModel(
                resolver.GetService<IFileService>(),
                resolver.GetService<IDirectoryService>(),
                resolver.GetService<IFilesSelectionService>(),
                resolver.GetService<IFileViewModelFactory>(),
                resolver.GetService<IFileSystemWatchingService>(),
                resolver.GetService<IApplicationDispatcher>()
                ));
            services.RegisterLazySingleton(() => new MainWindowViewModel(
                resolver.GetService<IDirectoryService>(),
                resolver.GetService<OperationsViewModel>(),
                resolver.GetService<FilesPanelViewModel>(),
                resolver.GetService<FilesPanelViewModel>()));
        }
    }
}