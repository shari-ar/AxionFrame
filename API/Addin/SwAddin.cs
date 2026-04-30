using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swpublished;
using SolidWorksTools;
using SolidWorksTools.File;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace AxionFrame
{
    /// <summary>
    /// Summary description for AxionFrame.
    /// </summary>
    [Guid("2f175406-8dd1-441a-b5f3-65e60ced575c"), ComVisible(true)]
    [SwAddin(
        Description = "AxionFrame description",
        Title = "AxionFrame",
        LoadAtStartup = true
        )]
    public partial class SwAddin : ISwAddin
    {
        #region Local Variables
        ISldWorks iSwApp = null;
        ICommandManager iCmdMgr = null;
        int addinID = 0;
        BitmapHandler iBmp;
        int registerID;

        public const int mainCmdGroupID = 5;
        public const int mainItemID1 = 0;
        public const int mainItemID2 = 1;
        public const int mainItemID3 = 2;
        public const int flyoutGroupID = 91;

        string[] mainIcons = new string[6];
        string[] icons = new string[6];

        #region Event Handler Variables
        Hashtable openDocs = new Hashtable();
        SolidWorks.Interop.sldworks.SldWorks SwEventPtr = null;
        #endregion

        #region Settings Page Variables
        public SettingsPage ppage = null;
        #endregion

        // Public Properties
        public ISldWorks SwApp
        {
            get { return iSwApp; }
        }

        public ICommandManager CmdMgr
        {
            get { return iCmdMgr; }
        }

        public Hashtable OpenDocs
        {
            get { return openDocs; }
        }

        #endregion

        #region ISwAddin Implementation
        public SwAddin()
        {
        }

        public bool ConnectToSW(object ThisSW, int cookie)
        {
            iSwApp = (ISldWorks)ThisSW;
            addinID = cookie;

            //Setup callbacks
            iSwApp.SetAddinCallbackInfo(0, this, addinID);

            #region Setup the Command Manager
            iCmdMgr = iSwApp.GetCommandManager(cookie);
            AddCommandMgr();
            #endregion

            #region Setup the Event Handlers
            SwEventPtr = (SolidWorks.Interop.sldworks.SldWorks)iSwApp;
            openDocs = new Hashtable();
            AttachEventHandlers();
            #endregion

            #region Setup Sample Property Manager
            AddSettings();
            #endregion

            return true;
        }

        public bool DisconnectFromSW()
        {
            RemoveCommandMgr();
            RemoveSettings();
            DetachEventHandlers();

            if (iCmdMgr != null)
            {
                Marshal.ReleaseComObject(iCmdMgr);
            }

            iCmdMgr = null;
            if (iSwApp != null)
            {
                Marshal.ReleaseComObject(iSwApp);
            }

            iSwApp = null;
            //The addin _must_ call GC.Collect() here in order to retrieve all managed code pointers
            GC.Collect();
            GC.WaitForPendingFinalizers();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return true;
        }
        #endregion
    }
}
