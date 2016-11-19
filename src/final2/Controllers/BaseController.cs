using Microsoft.AspNet.Mvc;
using System.Collections.Generic;

namespace Toys.Controllers
{
    public abstract class BaseController: Controller
    {
        public const string INFO_MSG_LIST = "msg";
        public const string ERROR_MSG_LIST = "errorMsg";

        public void AddError(string msg)
        {
            GetOrAddMsgList(ERROR_MSG_LIST).Add(msg);
        }

        public void AddInfo(string msg)
        {
            GetOrAddMsgList(INFO_MSG_LIST).Add(msg);
        }

        private List<string> GetOrAddMsgList(string listName)
        {
            object msgList;

            if (!TempData.TryGetValue(listName, out msgList))
            {
                msgList = new List<string>();
                TempData[listName] = msgList;
            }
            else
            {
                msgList = new List<string>(((IEnumerable<string>)msgList));
                TempData[listName] = msgList;
            }

            return (List<string>)msgList;
        }
    }
}
