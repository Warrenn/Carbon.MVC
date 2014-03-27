using System.Collections.Generic;
using System.IO;

namespace CarbonKnown.FileWatcherService
{
    public partial class FileWatcherConfigSection
    {
        public IDictionary<string, HandlerInfo> Handlers
        {
            get
            {
                var returnValue = new SortedDictionary<string, HandlerInfo>();
                foreach (GroupInstance instance in GroupInstances)
                {
                    var groupCollection = HandlerGroups.GetItemByKey(instance.GroupName);
                    if (groupCollection == null) continue;
                    foreach (GroupElement groupElement in groupCollection)
                    {
                        var folder = Path.Combine(instance.BaseFolder, groupElement.RelativeFolder);
                        var handler = new HandlerInfo
                            {
                                HandlerName = groupElement.HandlerName,
                                Host = instance.Host
                            };
                        returnValue[folder] = handler;
                    }
                }
                foreach (HandlerElement handlerElement in FileHandlers)
                {
                    returnValue[handlerElement.Folder] = new HandlerInfo
                        {
                            HandlerName = handlerElement.HandlerName,
                            Host = handlerElement.Host
                        };
                }
                return returnValue;
            }
        }

        public class  HandlerInfo
        {
            public string Host { get; set; }
            public string HandlerName { get; set; }
        }
    }
}
