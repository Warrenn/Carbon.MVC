using System;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using CarbonKnown.FileReaders;
using CarbonKnown.FileReaders.Constants;
using CarbonKnown.FileReaders.FileHandler;
using CarbonKnown.FileWatcherService.Properties;

namespace CarbonKnown.FileWatcherService
{
    public class FolderMonitor : IDisposable
    {
        private readonly IFileHandler fileHandler;
        private readonly string path;
        private IDisposable subscription;
        private FileSystemWatcher watcher;
        private IObservable<EventPattern<FileSystemEventArgs>> watcherObservable;
        private TimeSpan interval;
        private int retryCount;
        private IScheduler retryScheduler;

        public FolderMonitor(string path, IFileHandler fileHandler)
        {
            this.fileHandler = fileHandler;
            this.path = path;
        }

        public virtual void StartMonitoring()
        {
            if (watcher != null) return;
            watcher = new FileSystemWatcher(path)
            {
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.FileName
            };
            subscription = WatcherObservable.Subscribe(OnNext);
        }

        public  virtual void StopMonitoring()
        {
            subscription.Dispose(PolicyName.Disposable);
            watcher.Dispose(PolicyName.Disposable);
            subscription = null;
            watcher = null;
        }

        internal virtual IObservable<EventPattern<FileSystemEventArgs>> WatcherObservable
        {
            get
            {
                return watcherObservable ??
                       (watcherObservable =
                        Observable.FromEventPattern
                            <FileSystemEventHandler, FileSystemEventArgs>(
                                handler => watcher.Created += handler,
                                handler => watcher.Created -= handler));
            }
            set { watcherObservable = value; }
        }

        protected virtual void OnNext(EventPattern<FileSystemEventArgs> newFileEvent)
        {
            var fullPath = newFileEvent.EventArgs.FullPath;

            RetryScheduler.Schedule(0, TimeSpan.Zero, (state, recurse) =>
                {
                    Stream stream;
                    try
                    {
                        stream = File.OpenRead(fullPath);
                    }
                    catch (Exception ex)
                    {
                        Interlocked.Increment(ref state);
                        if (state < RetryCount)
                        {
                            recurse(state, Interval);
                            return;
                        }
                        fileHandler.ReportError(fullPath, ex);
                        return;
                    }
                    try
                    {
                        fileHandler.ProcessFile(fullPath, stream);
                    }
                    catch (Exception ex)
                    {
                        fileHandler.ReportError(fullPath, ex);
                    }
                    finally
                    {
                        stream.Dispose();
                    }
                });
        }

        internal virtual IScheduler RetryScheduler
        {
            get { return retryScheduler ?? (retryScheduler = Scheduler.Default); }
            set { retryScheduler = value; }
        }

        internal protected virtual TimeSpan Interval
        {
            get
            {
                if (interval == default(TimeSpan))
                {
                    interval = Settings.Default.Interval;
                }
                return interval;
            }
            set { interval = value; }
        }

        internal protected virtual int RetryCount
        {
            get
            {
                if (retryCount == default(int))
                {
                    retryCount = Settings.Default.RetryCount;
                }
                return retryCount;
            }
            set { retryCount = value; }
        }

        public virtual void Dispose()
        {
            fileHandler.Dispose(PolicyName.Disposable);
            StopMonitoring();
        }
    }
}
