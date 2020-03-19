using System;
using ApplicationDispatcher.Implementations;
using ApplicationDispatcher.Interfaces;
using Camelot.DataAccess.LiteDb;
using Camelot.DataAccess.UnitOfWork;
using Camelot.Factories.Implementations;
using Camelot.Factories.Interfaces;
using Camelot.FileSystemWatcherWrapper.Implementations;
using Camelot.FileSystemWatcherWrapper.Interfaces;
using Camelot.Mediator.Implementations;
using Camelot.Mediator.Interfaces;
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
            RegisterDataAccess(services, resolver);
            RegisterServices(services, resolver);
            RegisterViewModels(services, resolver);
        }

        private static void RegisterDataAccess(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IUnitOfWorkFactory>(() => new LiteDbUnitOfWorkFactory());
        }

        private static void RegisterServices(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IFileService>(() => new FileService());
            services.RegisterLazySingleton<IFileSystemWatcherWrapperFactory>(() => new FileSystemWatcherWrapperFactory());
            services.RegisterLazySingleton<ITaskPool>(() => new TaskPool.Implementations.TaskPool(Environment.ProcessorCount));
            services.Register<IOperationsFactory>(() => new OperationsFactory(
                resolver.GetService<ITaskPool>()
                ));
            services.RegisterLazySingleton<IFileSystemWatchingService>(() => new FileSystemWatchingService(
                resolver.GetService<IFileSystemWatcherWrapperFactory>()
                ));
            services.RegisterLazySingleton<IFilesSelectionService>(() => new FilesSelectionService());
            services.RegisterLazySingleton<IOperationsService>(() => new OperationsService(
                resolver.GetService<IFilesSelectionService>(),
                resolver.GetService<IOperationsFactory>(),
                resolver.GetService<IDirectoryService>(),
                resolver.GetService<IFileOpeningService>()
                ));
            services.RegisterLazySingleton<IDirectoryService>(() => new DirectoryService());
            services.RegisterLazySingleton<IProcessService>(() => new ProcessService());
            services.RegisterLazySingleton<IFileOpeningService>(() => new FileOpeningService(
                resolver.GetService<IProcessService>(),
                resolver.GetService<IPlatformService>()
                ));
            services.RegisterLazySingleton<IFileSystemWatcherWrapperFactory>(() => new FileSystemWatcherWrapperFactory());
            services.Register<IFileSystemWatchingService>(() => new FileSystemWatchingService(
                resolver.GetService<IFileSystemWatcherWrapperFactory>()
                ));
            services.RegisterLazySingleton(() => new FileOpeningBehavior(
                resolver.GetService<IFileOpeningService>()
                ));
            services.RegisterLazySingleton(() => new DirectoryOpeningBehavior(
                resolver.GetService<IDirectoryService>()
                ));
            services.RegisterLazySingleton<IApplicationDispatcher>(() => new AvaloniaDispatcher());
            services.RegisterLazySingleton<IFileSizeFormatter>(() => new FileSizeFormatter());
            services.RegisterLazySingleton<IPlatformService>(() => new PlatformService());
        }

        private static void RegisterViewModels(IMutableDependencyResolver services, IReadonlyDependencyResolver resolver)
        {
            services.RegisterLazySingleton<IFilesOperationsMediator>(() => new FilesOperationsMediator(
                resolver.GetService<IDirectoryService>(),
                resolver.GetService<IOperationsService>()
                ));
            services.RegisterLazySingleton<IFileViewModelFactory>(() => new FileViewModelFactory(
                resolver.GetService<FileOpeningBehavior>(),
                resolver.GetService<DirectoryOpeningBehavior>(),
                resolver.GetService<IFileSizeFormatter>()

                ));
            services.Register(() => new OperationsViewModel(
                resolver.GetService<IFilesOperationsMediator>()
                ));
            services.Register(() => new FilesPanelViewModel(
                resolver.GetService<IFileService>(),
                resolver.GetService<IDirectoryService>(),
                resolver.GetService<IFilesSelectionService>(),
                resolver.GetService<IFileViewModelFactory>(),
                resolver.GetService<IFileSystemWatchingService>(),
                resolver.GetService<IApplicationDispatcher>()
                ));
            services.RegisterLazySingleton(() => new MainWindowViewModel(
                resolver.GetService<IFilesOperationsMediator>(),
                resolver.GetService<OperationsViewModel>(),
                resolver.GetService<FilesPanelViewModel>(),
                resolver.GetService<FilesPanelViewModel>()
                ));
        }
    }
}